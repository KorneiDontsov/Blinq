namespace Blinq;

public struct CaptureIterator<T, TCapture, TIterator>: IIterator<(T Value, TCapture Capture)> where TIterator: IIterator<T> {
   TIterator Iterator;
   readonly TCapture Capture;

   public CaptureIterator (TIterator iterator, TCapture capture) {
      Iterator = iterator;
      Capture = capture;
   }

   public (T Value, TCapture Capture) Current => (Iterator.Current, Capture);

   public bool MoveNext () {
      return Iterator.MoveNext();
   }
}

public static partial class Sequence {
   /// <summary>Appends value to each element of a sequence.</summary>
   /// <param name="capture">A value to append to each element.</param>
   /// <typeparam name="TCapture">The type of <paramref name="capture" />.</typeparam>
   public static Sequence<(T Value, TCapture Capture), CaptureIterator<T, TCapture, TIterator>> Capture<T, TIterator, TCapture> (
      this in Sequence<T, TIterator> sequence,
      TCapture capture
   ) where TIterator: IIterator<T> {
      return new Sequence<(T value, TCapture capture), CaptureIterator<T, TCapture, TIterator>>(
         new CaptureIterator<T, TCapture, TIterator>(sequence.Iterator, capture),
         sequence.Count
      );
   }
}
