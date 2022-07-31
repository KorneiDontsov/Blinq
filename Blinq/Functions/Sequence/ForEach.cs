namespace Blinq;

readonly struct ForEachFoldFunc<T>: IFoldFunc<T, ValueTuple> {
   readonly Action<T> Action;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public ForEachFoldFunc (Action<T> action) {
      Action = action;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Invoke (T item, ref ValueTuple _) {
      Action(item);
      return false;
   }
}

public static partial class Sequence {
   /// <summary>Executes an action to each element of a sequence.</summary>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static void ForEach<T, TIterator> (this Sequence<T, TIterator> sequence, Action<T> action) where TIterator: IIterator<T> {
      sequence.Iterator.Fold(default(ValueTuple), new ForEachFoldFunc<T>(action));
   }
}
