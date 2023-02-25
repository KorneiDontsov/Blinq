using Blinq.Functors;

namespace Blinq;

public interface IDictionaryCollectPolicy {
   static abstract void Add<TKey, TValue> (Dictionary<TKey, TValue> dictionary, TKey key, TValue value) where TKey: notnull;
}

public readonly struct DictionaryCollector<T, TKey, TKeySelector, TValue, TValueSelector, TPolicy>: ICollector<T, Dictionary<TKey, TValue>>
where TKey: notnull
where TKeySelector: ISelector<T, TKey>
where TValueSelector: ISelector<T, TValue>
where TPolicy: IDictionaryCollectPolicy {
   readonly Dictionary<TKey, TValue> Dictionary;
   readonly TKeySelector KeySelector;
   readonly TValueSelector ValueSelector;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public DictionaryCollector (TKeySelector keySelector, TValueSelector valueSelector, IEqualityComparer<TKey>? equaler) {
      Dictionary = new(equaler);
      KeySelector = keySelector;
      ValueSelector = valueSelector;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void EnsureCapacity (int minCapacity) {
      Dictionary.EnsureCapacity(minCapacity);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void Add (T item) {
      TPolicy.Add(Dictionary, KeySelector.Invoke(item), ValueSelector.Invoke(item));
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public Dictionary<TKey, TValue> Build () {
      return Dictionary;
   }
}

public static partial class Collectors {
   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Contract<
      ICollector<KeyValuePair<TKey, TValue>, Dictionary<TKey, TValue>>,
      DictionaryCollector<
         KeyValuePair<TKey, TValue>,
         TKey,
         KeyValuePairKeySelector<TKey, TValue>,
         TValue,
         KeyValuePairValueSelector<TKey, TValue>,
         TPolicy>
   > Dictionary<TKey, TValue, TPolicy> (
      this CollectorProvider<KeyValuePair<TKey, TValue>> collectorProvider,
      IEqualityComparer<TKey>? equaler = null,
      Type<TPolicy> tPolicy = default
   )
   where TKey: notnull
   where TPolicy: IDictionaryCollectPolicy {
      _ = collectorProvider;
      _ = tPolicy;
      return new DictionaryCollector<
         KeyValuePair<TKey, TValue>,
         TKey,
         KeyValuePairKeySelector<TKey, TValue>,
         TValue,
         KeyValuePairValueSelector<TKey, TValue>,
         TPolicy
      >(
         new KeyValuePairKeySelector<TKey, TValue>(),
         new KeyValuePairValueSelector<TKey, TValue>(),
         equaler
      );
   }

   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Contract<
      ICollector<KeyValuePair<TKey, TValue>, Dictionary<TKey, TValue>>,
      DictionaryCollector<
         KeyValuePair<TKey, TValue>,
         TKey,
         KeyValuePairKeySelector<TKey, TValue>,
         TValue,
         KeyValuePairValueSelector<TKey, TValue>,
         CollectPolicies.AddOrSkip>
   > Dictionary<TKey, TValue> (this CollectorProvider<KeyValuePair<TKey, TValue>> collectorProvider, IEqualityComparer<TKey>? equaler = null)
   where TKey: notnull {
      return collectorProvider.Dictionary(equaler, CollectPolicy.AddOrSkip);
   }

   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Contract<
      ICollector<T, Dictionary<TKey, T>>,
      DictionaryCollector<T, TKey, FuncSelector<T, TKey>, T, ItselfSelector<T>, TPolicy>
   > Dictionary<T, TKey, TPolicy> (
      this CollectorProvider<T> collectorProvider,
      Func<T, TKey> keySelector,
      IEqualityComparer<TKey>? equaler = null,
      Type<TPolicy> tPolicy = default
   )
   where TKey: notnull
   where TPolicy: IDictionaryCollectPolicy {
      _ = tPolicy;
      return new DictionaryCollector<T, TKey, FuncSelector<T, TKey>, T, ItselfSelector<T>, TPolicy>(
         new FuncSelector<T, TKey>(keySelector),
         new ItselfSelector<T>(),
         equaler
      );
   }

   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Contract<
      ICollector<T, Dictionary<TKey, T>>,
      DictionaryCollector<T, TKey, FuncSelector<T, TKey>, T, ItselfSelector<T>, CollectPolicies.AddOrSkip>
   > Dictionary<T, TKey> (this CollectorProvider<T> collectorProvider, Func<T, TKey> keySelector, IEqualityComparer<TKey>? equaler = null)
   where TKey: notnull {
      return collectorProvider.Dictionary(keySelector, equaler, CollectPolicy.AddOrSkip);
   }

   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Contract<
      ICollector<T, Dictionary<TKey, TValue>>,
      DictionaryCollector<T, TKey, FuncSelector<T, TKey>, TValue, FuncSelector<T, TValue>, TPolicy>
   > Dictionary<T, TKey, TValue, TPolicy> (
      this CollectorProvider<T> collectorProvider,
      Func<T, TKey> keySelector,
      Func<T, TValue> valueSelector,
      IEqualityComparer<TKey>? equaler = null,
      Type<TPolicy> tPolicy = default
   )
   where TKey: notnull
   where TPolicy: IDictionaryCollectPolicy {
      _ = collectorProvider;
      _ = tPolicy;
      return new DictionaryCollector<T, TKey, FuncSelector<T, TKey>, TValue, FuncSelector<T, TValue>, TPolicy>(
         new FuncSelector<T, TKey>(keySelector),
         new FuncSelector<T, TValue>(valueSelector),
         equaler
      );
   }

   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Contract<
      ICollector<T, Dictionary<TKey, TValue>>,
      DictionaryCollector<T, TKey, FuncSelector<T, TKey>, TValue, FuncSelector<T, TValue>, CollectPolicies.AddOrSkip>
   > Dictionary<T, TKey, TValue> (
      this CollectorProvider<T> collectorProvider,
      Func<T, TKey> keySelector,
      Func<T, TValue> valueSelector,
      IEqualityComparer<TKey>? equaler = null
   ) where TKey: notnull {
      return collectorProvider.Dictionary(keySelector, valueSelector, equaler, CollectPolicy.AddOrSkip);
   }
}
