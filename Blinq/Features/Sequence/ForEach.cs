namespace Blinq;

readonly struct ForEachFold<T>: IFold<T, ValueTuple> {
   readonly Action<T> Action;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public ForEachFold (Action<T> action) {
      Action = action;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Invoke (T item, ref ValueTuple _) {
      Action(item);
      return false;
   }
}

public static partial class Iterator {
   /// <summary>Executes an action to each element of a sequence.</summary>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static void ForEach<T, TIterator> (this in Contract<IIterator<T>, TIterator> iterator, Action<T> action) where TIterator: IIterator<T> {
      iterator.Value.Fold(default(ValueTuple), new ForEachFold<T>(action));
   }
}
