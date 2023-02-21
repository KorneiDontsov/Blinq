namespace Blinq.Collections;

public interface ITableEntry<TKey> where TKey: notnull {
   public TKey Key { get; }
}
