namespace Blinq.Collections;

public interface ITableKeySelector<T, TKey> where TKey: notnull {
   static abstract TKey SelectKey (in T entry);
}
