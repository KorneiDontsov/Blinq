using System.Diagnostics.CodeAnalysis;

namespace Blinq;

struct CastUpFoldFunc<TFrom, TAccumulator, TTo, TInnerFoldFunc>: IFoldFunc<TFrom, TAccumulator>
where TFrom: TTo
where TInnerFoldFunc: IFoldFunc<TTo, TAccumulator> {
   TInnerFoldFunc InnerFoldFunc;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public CastUpFoldFunc (TInnerFoldFunc innerFoldFunc) {
      InnerFoldFunc = innerFoldFunc;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Invoke (TFrom item, ref TAccumulator accumulator) {
      return InnerFoldFunc.Invoke(item, ref accumulator);
   }
}

struct CastDownFoldFunc<TFrom, TAccumulator, TTo, TInnerFoldFunc>: IFoldFunc<TFrom, TAccumulator>
where TTo: TFrom
where TInnerFoldFunc: IFoldFunc<TTo, TAccumulator> {
   TInnerFoldFunc InnerFoldFunc;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public CastDownFoldFunc (TInnerFoldFunc innerFoldFunc) {
      InnerFoldFunc = innerFoldFunc;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Invoke (TFrom item, ref TAccumulator accumulator) {
      return InnerFoldFunc.Invoke((TTo)item!, ref accumulator);
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

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public TAccumulator Fold<TAccumulator, TFoldFunc> (TAccumulator seed, TFoldFunc func) where TFoldFunc: IFoldFunc<TTo, TAccumulator> {
      return FromIterator.Fold(seed, new CastUpFoldFunc<TFrom, TAccumulator, TTo, TFoldFunc>(func));
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

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public TAccumulator Fold<TAccumulator, TFoldFunc> (TAccumulator seed, TFoldFunc func) where TFoldFunc: IFoldFunc<TTo, TAccumulator> {
      return FromIterator.Fold(seed, new CastDownFoldFunc<TFrom, TAccumulator, TTo, TFoldFunc>(func));
   }
}

public interface ICastType { }

public static class CastType {
   public readonly struct Up: ICastType { }

   public readonly struct Down: ICastType { }
}

[SuppressMessage("ReSharper", "UnusedTypeParameter")]
public readonly struct CastSyntaxOutput<TFrom, TTo, TCastType> where TCastType: ICastType { }

public readonly struct CastSyntaxInput<TFrom> {
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public CastSyntaxOutput<TFrom, TTo, CastType.Up> Up<TTo> () {
      return default;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public CastSyntaxOutput<TFrom, TTo, CastType.Down> Down<TTo> () {
      return default;
   }
}

public delegate CastSyntaxOutput<TFrom, TTo, TCastType> CastSyntax<TFrom, TTo, TCastType> (CastSyntaxInput<TFrom> input) where TCastType: ICastType;

public static partial class Sequence {
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Sequence<TTo, CastUpIterator<TTo, T, TIterator>> Cast<T, TIterator, TTo> (
      this in Sequence<T, TIterator> sequence,
      CastSyntax<T, TTo, CastType.Up> syntax
   )
   where T: TTo
   where TIterator: IIterator<T> {
      return new Sequence<TTo, CastUpIterator<TTo, T, TIterator>>(
         new CastUpIterator<TTo, T, TIterator>(sequence.Iterator),
         sequence.Count
      );
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Sequence<TTo, CastDownIterator<TTo, T, TIterator>> Cast<T, TIterator, TTo> (
      this in Sequence<T, TIterator> sequence,
      CastSyntax<T, TTo, CastType.Down> syntax
   )
   where TIterator: IIterator<T>
   where TTo: T {
      return new Sequence<TTo, CastDownIterator<TTo, T, TIterator>>(
         new CastDownIterator<TTo, T, TIterator>(sequence.Iterator),
         sequence.Count
      );
   }
}
