namespace Blinq.Collections;

readonly struct SetKeySelector<T>: ITableKeySelector<T, T> where T: notnull {
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static T SelectKey (in T entry) {
      return entry;
   }
}
