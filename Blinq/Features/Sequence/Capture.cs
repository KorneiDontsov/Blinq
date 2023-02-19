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

struct CaptureFold<T, TAccumulator, TCapture, TInnerFold>: IFold<T, TAccumulator>
where TInnerFold: IFold<ItemWithCapture<T, TCapture>, TAccumulator> {
   readonly TCapture Capture;
   TInnerFold InnerFold;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public CaptureFold (TCapture capture, TInnerFold innerFold) {
      Capture = capture;
      InnerFold = innerFold;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Invoke (T item, ref TAccumulator accumulator) {
      return InnerFold.Invoke(new ItemWithCapture<T, TCapture>(item, Capture), ref accumulator);
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
   public bool TryPop (out ItemWithCapture<T, TCapture> item) {
      if (Iterator.TryPop(out var underlyingItem)) {
         item = new ItemWithCapture<T, TCapture>(underlyingItem, Capture);
         return true;
      } else {
         item = default;
         return false;
      }
   }

   /// <inheritdoc />
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public TAccumulator Fold<TAccumulator, TFold> (TAccumulator seed, TFold fold)
   where TFold: IFold<ItemWithCapture<T, TCapture>, TAccumulator> {
      return Iterator.Fold(seed, new CaptureFold<T, TAccumulator, TCapture, TFold>(Capture, fold));
   }

   /// <inheritdoc />
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool TryGetCount (out int count) {
      return Iterator.TryGetCount(out count);
   }
}

public static partial class Iterator {
   /// <summary>Appends value to each element of a sequence.</summary>
   /// <param name="capture">A value to append to each element.</param>
   /// <typeparam name="TCapture">The type of <paramref name="capture" />.</typeparam>
   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Contract<IIterator<ItemWithCapture<T, TCapture>>, CaptureIterator<T, TCapture, TIterator>> Capture<T, TIterator, TCapture> (
      this in Contract<IIterator<T>, TIterator> iterator,
      TCapture capture
   ) where TIterator: IIterator<T> {
      return new CaptureIterator<T, TCapture, TIterator>(iterator, capture);
   }
}
