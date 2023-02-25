namespace Blinq.Collections;

interface ISetKeySelector<T>: ITableKeySelector<T, T> where T: notnull {
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   static T ITableKeySelector<T, T>.SelectKey (in T entry) {
      return entry;
   }
}
