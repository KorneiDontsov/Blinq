namespace Blinq;

readonly struct CastUpFold<TFrom, TAccumulator, TTo, TInnerFold>: IFold<TFrom, TAccumulator>
where TFrom: TTo
where TInnerFold: IFold<TTo, TAccumulator> {
   readonly TInnerFold InnerFold;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public CastUpFold (TInnerFold innerFold) {
      InnerFold = innerFold;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Invoke (TFrom item, ref TAccumulator accumulator) {
      return InnerFold.Invoke(item, ref accumulator);
   }
}

readonly struct CastDownFold<TFrom, TAccumulator, TTo, TInnerFold>: IFold<TFrom, TAccumulator>
where TTo: TFrom
where TInnerFold: IFold<TTo, TAccumulator> {
   readonly TInnerFold InnerFold;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public CastDownFold (TInnerFold innerFold) {
      InnerFold = innerFold;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Invoke (TFrom item, ref TAccumulator accumulator) {
      return InnerFold.Invoke((TTo)item!, ref accumulator);
   }
}

public struct CastUpIterator<TTo, TFrom, TFromIterator>: IIterator<TTo>
where TFrom: TTo
where TFromIterator: IIterator<TFrom> {
   TFromIterator FromIterator;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public CastUpIterator (TFromIterator fromIterator) {
      FromIterator = fromIterator;
   }

   /// <inheritdoc />
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool TryPop ([MaybeNullWhen(false)] out TTo item) {
      // TODO: Try to optimize: if (default(TFrom) is null) ... out Unsafe.As<TTo?, TFrom?>(ref item)
      if (FromIterator.TryPop(out var actualItem)) {
         item = actualItem;
         return true;
      } else {
         item = default;
         return false;
      }
   }

   /// <inheritdoc />
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public TAccumulator Fold<TAccumulator, TFold> (TAccumulator accumulator, TFold fold) where TFold: IFold<TTo, TAccumulator> {
      return FromIterator.Fold(accumulator, new CastUpFold<TFrom, TAccumulator, TTo, TFold>(fold));
   }

   /// <inheritdoc />
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool TryGetCount (out int count) {
      return FromIterator.TryGetCount(out count);
   }
}

public struct CastDownIterator<TTo, TFrom, TFromIterator>: IIterator<TTo>
where TTo: TFrom
where TFromIterator: IIterator<TFrom> {
   TFromIterator FromIterator;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public CastDownIterator (TFromIterator fromIterator) {
      FromIterator = fromIterator;
   }

   /// <inheritdoc />
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool TryPop ([MaybeNullWhen(false)] out TTo item) {
      // TODO: Try to optimize: if (default(TTo) is null) ... out Unsafe.As<TTo?, TFrom?>(ref item)
      if (FromIterator.TryPop(out var actualItem)) {
         item = (TTo)actualItem!;
         return true;
      } else {
         item = default;
         return false;
      }
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public TAccumulator Fold<TAccumulator, TFold> (TAccumulator accumulator, TFold fold) where TFold: IFold<TTo, TAccumulator> {
      return FromIterator.Fold(accumulator, new CastDownFold<TFrom, TAccumulator, TTo, TFold>(fold));
   }

   /// <inheritdoc />
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool TryGetCount (out int count) {
      return FromIterator.TryGetCount(out count);
   }
}

[ReadOnly(true)]
public interface ICastType { }

public static class CastType {
   public readonly struct Up: ICastType { }

   public readonly struct Down: ICastType { }
}

[SuppressMessage("ReSharper", "UnusedTypeParameter")]
public readonly struct CastSyntaxOutput<TFrom, TTo, TCastType> where TCastType: ICastType { }

public readonly struct CastSyntaxInput<TFrom> {
   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public CastSyntaxOutput<TFrom, TTo, CastType.Up> Up<TTo> () {
      return default;
   }

   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public CastSyntaxOutput<TFrom, TTo, CastType.Down> Down<TTo> () {
      return default;
   }
}

[Pure]
public delegate CastSyntaxOutput<TFrom, TTo, TCastType> CastSyntax<TFrom, TTo, TCastType> (CastSyntaxInput<TFrom> input) where TCastType: ICastType;

public static partial class Iterator {
   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Contract<IIterator<TTo>, CastUpIterator<TTo, T, TIterator>> Cast<T, TIterator, TTo> (
      this in Contract<IIterator<T>, TIterator> iterator,
      CastSyntax<T, TTo, CastType.Up> syntax
   )
   where T: TTo
   where TIterator: IIterator<T> {
      _ = syntax;
      return new CastUpIterator<TTo, T, TIterator>(iterator);
   }

   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Contract<IIterator<TTo>, CastDownIterator<TTo, T, TIterator>> Cast<T, TIterator, TTo> (
      this in Contract<IIterator<T>, TIterator> iterator,
      CastSyntax<T, TTo, CastType.Down> syntax
   )
   where TIterator: IIterator<T>
   where TTo: T {
      _ = syntax;
      return new CastDownIterator<TTo, T, TIterator>(iterator);
   }
}
