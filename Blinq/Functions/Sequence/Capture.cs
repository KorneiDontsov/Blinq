using System.Runtime.CompilerServices;

namespace Blinq;

struct CaptureAccumulator<T, TAccumulated, TCapture, TNextAccumulator>: IAccumulator<T, TAccumulated>
where TNextAccumulator: IAccumulator<(T Value, TCapture Capture), TAccumulated> {
   readonly TCapture Capture;
   TNextAccumulator NextAccumulator;

   public CaptureAccumulator (TCapture capture, TNextAccumulator nextAccumulator) {
      Capture = capture;
      NextAccumulator = nextAccumulator;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Invoke (T item, ref TAccumulated accumulated) {
      return NextAccumulator.Invoke((item, Capture), ref accumulated);
   }
}

public struct CaptureIterator<T, TCapture, TIterator>: IIterator<(T Value, TCapture Capture)> where TIterator: IIterator<T> {
   TIterator Iterator;
   readonly TCapture Capture;

   public CaptureIterator (TIterator iterator, TCapture capture) {
      Iterator = iterator;
      Capture = capture;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public TAccumulated Accumulate<TAccumulated, TAccumulator> (TAccumulator accumulator, TAccumulated seed)
   where TAccumulator: IAccumulator<(T Value, TCapture Capture), TAccumulated> {
      return Iterator.Accumulate(new CaptureAccumulator<T, TAccumulated, TCapture, TAccumulator>(Capture, accumulator), seed);
   }
}

public static partial class Sequence {
   /// <summary>Appends value to each element of a sequence.</summary>
   /// <param name="capture">A value to append to each element.</param>
   /// <typeparam name="TCapture">The type of <paramref name="capture" />.</typeparam>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
