using System.Buffers;
using System.Runtime.InteropServices;

namespace Blinq.Collections;

interface IPoolingVectorTrait<T, TRaw> {
   [Pure] static abstract bool ClearArray { get; }
   [Pure] static abstract int ItemSize { get; }
   [Pure] static abstract ref T At (TRaw[] items, int index);
   [Pure] static abstract Span<T> AsSpan (TRaw[] items, int size);
}

sealed class ReferencePoolingVectorTrait<T>: IPoolingVectorTrait<T, object?> {
   public static bool ClearArray => true;
   public static int ItemSize => 1;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static ref T At (object?[] items, int index) {
      return ref Unsafe.As<object?, T>(ref items[index]);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Span<T> AsSpan (object?[] items, int size) {
      return MemoryMarshal.CreateSpan(ref At(items, 0), size);
   }
}

sealed class UnmanagedPoolingVectorTrait<T>: IPoolingVectorTrait<T, byte> {
   public static bool ClearArray => false;
   public static int ItemSize => Unsafe.SizeOf<T>();

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static ref T AtHead (byte[] items) {
      return ref Unsafe.As<byte, T>(ref items[0]);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static ref T At (byte[] items, int index) {
      return ref Unsafe.Add(ref AtHead(items), index);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Span<T> AsSpan (byte[] items, int size) {
      return MemoryMarshal.CreateSpan(ref AtHead(items), size);
   }
}

[StructLayout(LayoutKind.Sequential)]
struct PoolingVectorImpl<T, TRaw, TTrait> where TTrait: IPoolingVectorTrait<T, TRaw> {
   const int DefaultCapacity = 16;

   TRaw[] Items = Array.Empty<TRaw>();
   int Size;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public PoolingVectorImpl () { }

   public void Dispose () {
      ArrayPool<TRaw>.Shared.Return(Items, TTrait.ClearArray);
      this = new();
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   static int GetCapacity (TRaw[] items) {
      return items.Length / TTrait.ItemSize;
   }

   public int Capacity {
      set {
         var items = Items;
         var size = Size;

         if (value < size) Get.Throw<ArgumentOutOfRangeException>();

         var newLength = TTrait.ItemSize * value;
         if (newLength != items.Length) {
            if (newLength > 0) {
               var newItems = ArrayPool<TRaw>.Shared.Rent(newLength);
               if (size > 0) Array.Copy(items, newItems, TTrait.ItemSize * size);
               Items = newItems;
               ArrayPool<TRaw>.Shared.Return(items, TTrait.ClearArray);
            } else {
               Items = Array.Empty<TRaw>();
            }
         }
      }
   }

   [MethodImpl(MethodImplOptions.NoInlining)]
   void AddWithResize (T item) {
      var capacity = GetCapacity(Items);
      var newCapacity = capacity == 0 ? DefaultCapacity : 2 * capacity;
      if ((uint)newCapacity > int.MaxValue) newCapacity = int.MaxValue;
      Capacity = newCapacity;

      TTrait.At(Items, Size++) = item;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void Add (T item) {
      var items = Items;
      var size = Size;
      if (size < GetCapacity(items)) {
         Size = size + 1;
         TTrait.At(items, size) = item;
      } else {
         AddWithResize(item);
      }
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public readonly void CopyTo (T[] array, int arrayIndex) {
      TTrait.AsSpan(Items, Size).CopyTo(array.AsSpan(arrayIndex));
   }

   readonly T[] ToArray () {
      var size = Size;
      if (size > 0) {
         var array = new T[size];
         TTrait.AsSpan(Items, size).CopyTo(array);
         return array;
      } else {
         return Array.Empty<T>();
      }
   }

   public T[] MoveToArray () {
      var result = ToArray();
      Dispose();
      return result;
   }
}

static class PoolingVectorImpls {
   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static ref PoolingVectorImpl<T, object?, ReferencePoolingVectorTrait<T>> ReferenceImpl<T> (this ref PoolingVector<T> self) {
      return ref Unsafe.As<PoolingVector<T>, PoolingVectorImpl<T, object?, ReferencePoolingVectorTrait<T>>>(ref self);
   }

   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static ref PoolingVectorImpl<T, byte, UnmanagedPoolingVectorTrait<T>> UnmanagedImpl<T> (this ref PoolingVector<T> self) {
      return ref Unsafe.As<PoolingVector<T>, PoolingVectorImpl<T, byte, UnmanagedPoolingVectorTrait<T>>>(ref self);
   }

   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static ref ValueVector<T> ComplexImpl<T> (this ref PoolingVector<T> self) {
      return ref Unsafe.As<PoolingVector<T>, ValueVector<T>>(ref self);
   }
}

[SuppressMessage("ReSharper", "PrivateFieldCanBeConvertedToLocalVariable")]
[SuppressMessage("ReSharper", "FieldCanBeMadeReadOnly.Local")]
[SuppressMessage("ReSharper", "ConvertToAutoProperty")]
[StructLayout(LayoutKind.Sequential)]
struct PoolingVector<T>: IDisposable {
   object? Items;
   int Size;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public PoolingVector () {
      switch (Get<T>.MemoryKind) {
         case TypeMemoryKind.Reference: {
            this.ReferenceImpl() = new();
            break;
         }
         case TypeMemoryKind.Unmanaged: {
            this.UnmanagedImpl() = new();
            break;
         }
         case TypeMemoryKind.Complex: {
            this.ComplexImpl() = new();
            break;
         }
      }
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void Dispose () {
      switch (Get<T>.MemoryKind) {
         case TypeMemoryKind.Reference: {
            this.ReferenceImpl().Dispose();
            break;
         }
         case TypeMemoryKind.Unmanaged: {
            this.UnmanagedImpl().Dispose();
            break;
         }
         case TypeMemoryKind.Complex: {
            this.ComplexImpl() = new();
            break;
         }
      }
   }

   public readonly int Count => Size;

   public int Capacity {
      [MethodImpl(MethodImplOptions.AggressiveInlining)] set {
         switch (Get<T>.MemoryKind) {
            case TypeMemoryKind.Reference: {
               this.ReferenceImpl().Capacity = value;
               break;
            }
            case TypeMemoryKind.Unmanaged: {
               this.UnmanagedImpl().Capacity = value;
               break;
            }
            case TypeMemoryKind.Complex: {
               this.ComplexImpl().Capacity = value;
               break;
            }
         }
      }
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void Add (T item) {
      switch (Get<T>.MemoryKind) {
         case TypeMemoryKind.Reference: {
            this.ReferenceImpl().Add(item);
            break;
         }
         case TypeMemoryKind.Unmanaged: {
            this.UnmanagedImpl().Add(item);
            break;
         }
         case TypeMemoryKind.Complex: {
            this.ComplexImpl().Add(item);
            break;
         }
      }
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void CopyTo (T[] array, int arrayIndex) {
      switch (Get<T>.MemoryKind) {
         case TypeMemoryKind.Reference: {
            this.ReferenceImpl().CopyTo(array, arrayIndex);
            break;
         }
         case TypeMemoryKind.Unmanaged: {
            this.UnmanagedImpl().CopyTo(array, arrayIndex);
            break;
         }
         case TypeMemoryKind.Complex: {
            this.ComplexImpl().CopyTo(array, arrayIndex);
            break;
         }
      }
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public T[] MoveToArray () {
      return Get<T>.MemoryKind switch {
         TypeMemoryKind.Reference => this.ReferenceImpl().MoveToArray(),
         TypeMemoryKind.Unmanaged => this.UnmanagedImpl().MoveToArray(),
         TypeMemoryKind.Complex => this.ComplexImpl().MoveToArray(),
      };
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public ValueVector<T> MoveToValueVector () {
      return Get<T>.MemoryKind switch {
         TypeMemoryKind.Reference => new ValueVector<T>(this.ReferenceImpl().MoveToArray()),
         TypeMemoryKind.Unmanaged => new ValueVector<T>(this.UnmanagedImpl().MoveToArray()),
         TypeMemoryKind.Complex => this.ComplexImpl().Exchange(new()),
      };
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public ImmVector<T> MoveToImmutableVector () {
      return ImmVector<T>.Create(MoveToValueVector());
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public MutVector<T> MoveToMutableVector () {
      return new(MoveToValueVector());
   }
}
