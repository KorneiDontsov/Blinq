namespace Blinq;

public readonly struct ItemWithCapture<TItem, TCapture> {
   public readonly TItem Value;
   public readonly TCapture Capture;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public ItemWithCapture (TItem value, TCapture capture) {
      Value = value;
      Capture = capture;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void Deconstruct (out TItem value, out TCapture capture) {
      value = Value;
      capture = Capture;
   }
}

struct CaptureFoldFunc<T, TAccumulator, TCapture, TInnerFoldFunc>: IFoldFunc<T, TAccumulator>
where TInnerFoldFunc: IFoldFunc<ItemWithCapture<T, TCapture>, TAccumulator> {
   readonly TCapture Capture;
   TInnerFoldFunc InnerFoldFunc;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public CaptureFoldFunc (TCapture capture, TInnerFoldFunc innerFoldFunc) {
      Capture = capture;
      InnerFoldFunc = innerFoldFunc;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Invoke (T item, ref TAccumulator accumulator) {
      return InnerFoldFunc.Invoke(new ItemWithCapture<T, TCapture>(item, Capture), ref accumulator);
   }
}

public struct CaptureIterator<T, TCapture, TIterator>: IIterator<ItemWithCapture<T, TCapture>> where TIterator: IIterator<T> {
   TIterator Iterator;
   readonly TCapture Capture;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public CaptureIterator (TIterator iterator, TCapture capture) {
      Iterator = iterator;
      Capture = capture;
   }

   /// <inheritdoc />
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public TAccumulator Fold<TAccumulator, TFoldFunc> (TAccumulator seed, TFoldFunc func)
   where TFoldFunc: IFoldFunc<ItemWithCapture<T, TCapture>, TAccumulator> {
      return Iterator.Fold(seed, new CaptureFoldFunc<T, TAccumulator, TCapture, TFoldFunc>(Capture, func));
   }
}

public static partial class Sequence {
   /// <summary>Appends value to each element of a sequence.</summary>
   /// <param name="capture">A value to append to each element.</param>
   /// <typeparam name="TCapture">The type of <paramref name="capture" />.</typeparam>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Sequence<ItemWithCapture<T, TCapture>, CaptureIterator<T, TCapture, TIterator>> Capture<T, TIterator, TCapture> (
      this in Sequence<T, TIterator> sequence,
      TCapture capture
   ) where TIterator: IIterator<T> {
      return new Sequence<ItemWithCapture<T, TCapture>, CaptureIterator<T, TCapture, TIterator>>(
         new CaptureIterator<T, TCapture, TIterator>(sequence.Iterator, capture),
         sequence.Count
      );
   }
}
