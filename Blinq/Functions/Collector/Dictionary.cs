using Blinq.Functors;

namespace Blinq;

[ReadOnly(true)]
public interface IDictionaryAddFunc<TKey, TValue> {
   void Invoke (Dictionary<TKey, TValue> dictionary, TKey key, TValue value);
}

public readonly struct DictionaryAddOrExceptionFunc<TKey, TValue>: IDictionaryAddFunc<TKey, TValue> {
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void Invoke (Dictionary<TKey, TValue> dictionary, TKey key, TValue value) {
      dictionary.Add(key, value);
   }
}

public readonly struct DictionaryTryAddFunc<TKey, TValue>: IDictionaryAddFunc<TKey, TValue> {
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void Invoke (Dictionary<TKey, TValue> dictionary, TKey key, TValue value) {
      dictionary.TryAdd(key, value);
   }
}

public readonly struct DictionaryAddOrReplaceFunc<TKey, TValue>: IDictionaryAddFunc<TKey, TValue> {
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void Invoke (Dictionary<TKey, TValue> dictionary, TKey key, TValue value) {
      dictionary[key] = value;
   }
}

public readonly struct DictionaryAddFuncProvider<TKey, TValue> {
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public DictionaryAddOrExceptionFunc<TKey, TValue> AddOrException () {
      return new();
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public DictionaryTryAddFunc<TKey, TValue> TryAdd () {
      return new();
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public DictionaryAddOrReplaceFunc<TKey, TValue> AddOrReplace () {
      return new();
   }
}

[SuppressMessage("ReSharper", "TypeParameterCanBeVariant")]
public delegate TDictionaryAdd ProvideDictionaryAddFunc<TKey, TValue, TDictionaryAdd> (DictionaryAddFuncProvider<TKey, TValue> addFuncProvider);

public struct DictionaryCollector<T, TKey, TKeySelector, TValue, TValueSelector, TAddFunc>:
   ICollector<T, Dictionary<TKey, TValue>, Dictionary<TKey, TValue>>
where TKeySelector: ISelector<T, TKey>
where TValueSelector: ISelector<T, TValue>
where TAddFunc: IDictionaryAddFunc<TKey, TValue> {
   TKeySelector KeySelector;
   TValueSelector ValueSelector;
   TAddFunc AddFunc;
   readonly IEqualityComparer<TKey>? Equaler;

   public DictionaryCollector (TKeySelector keySelector, TValueSelector valueSelector, TAddFunc addFunc, IEqualityComparer<TKey>? equaler) {
      KeySelector = keySelector;
      ValueSelector = valueSelector;
      AddFunc = addFunc;
      Equaler = equaler;
   }

   public Dictionary<TKey, TValue> CreateBuilder (int expectedCapacity = 0) {
      return new Dictionary<TKey, TValue>(expectedCapacity, Equaler);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void Add (ref Dictionary<TKey, TValue> dictionary, T item) {
      AddFunc.Invoke(dictionary, KeySelector.Invoke(item), ValueSelector.Invoke(item));
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public readonly Dictionary<TKey, TValue> Build (ref Dictionary<TKey, TValue> dictionary) {
      return dictionary;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void Finalize (ref Dictionary<TKey, TValue> builder) { }
}

public static partial class Collector {
   public static TAddFunc Invoke<TKey, TValue, TAddFunc> (this ProvideDictionaryAddFunc<TKey, TValue, TAddFunc> provideAddFunc) {
      return provideAddFunc.Invoke(new DictionaryAddFuncProvider<TKey, TValue>());
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Use<
      ICollector<KeyValuePair<TKey, TValue>, Dictionary<TKey, TValue>, Dictionary<TKey, TValue>>,
      DictionaryCollector<
         KeyValuePair<TKey, TValue>,
         TKey,
         KeyValuePairKeySelector<TKey, TValue>,
         TValue,
         KeyValuePairValueSelector<TKey, TValue>,
         TAddFunc>
   > Dictionary<TKey, TValue, TAddFunc> (
      this CollectorProvider<KeyValuePair<TKey, TValue>> _,
      ProvideDictionaryAddFunc<TKey, TValue, TAddFunc> provideAddFunc,
      IEqualityComparer<TKey>? equaler = null
   )
   where TAddFunc: IDictionaryAddFunc<TKey, TValue> {
      return new DictionaryCollector<
         KeyValuePair<TKey, TValue>,
         TKey,
         KeyValuePairKeySelector<TKey, TValue>,
         TValue,
         KeyValuePairValueSelector<TKey, TValue>,
         TAddFunc
      >(
         new KeyValuePairKeySelector<TKey, TValue>(),
         new KeyValuePairValueSelector<TKey, TValue>(),
         provideAddFunc.Invoke(),
         equaler
      );
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Use<
      ICollector<KeyValuePair<TKey, TValue>, Dictionary<TKey, TValue>, Dictionary<TKey, TValue>>,
      DictionaryCollector<
         KeyValuePair<TKey, TValue>,
         TKey,
         KeyValuePairKeySelector<TKey, TValue>,
         TValue,
         KeyValuePairValueSelector<TKey, TValue>,
         DictionaryAddOrExceptionFunc<TKey, TValue>>
   > Dictionary<TKey, TValue> (this CollectorProvider<KeyValuePair<TKey, TValue>> _, IEqualityComparer<TKey>? equaler = null) {
      return new DictionaryCollector<
         KeyValuePair<TKey, TValue>,
         TKey,
         KeyValuePairKeySelector<TKey, TValue>,
         TValue,
         KeyValuePairValueSelector<TKey, TValue>,
         DictionaryAddOrExceptionFunc<TKey, TValue>
      >(
         new KeyValuePairKeySelector<TKey, TValue>(),
         new KeyValuePairValueSelector<TKey, TValue>(),
         new DictionaryAddOrExceptionFunc<TKey, TValue>(),
         equaler
      );
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Use<
      ICollector<T, Dictionary<TKey, T>, Dictionary<TKey, T>>,
      DictionaryCollector<T, TKey, FuncSelector<T, TKey>, T, ItselfSelector<T>, TAddFunc>
   > Dictionary<T, TKey, TAddFunc> (
      this CollectorProvider<T> _,
      Func<T, TKey> keySelector,
      ProvideDictionaryAddFunc<TKey, T, TAddFunc> provideAddFunc,
      IEqualityComparer<TKey>? equaler = null
   ) where TAddFunc: IDictionaryAddFunc<TKey, T> {
      return new DictionaryCollector<T, TKey, FuncSelector<T, TKey>, T, ItselfSelector<T>, TAddFunc>(
         new FuncSelector<T, TKey>(keySelector),
         new ItselfSelector<T>(),
         provideAddFunc.Invoke(),
         equaler
      );
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Use<
      ICollector<T, Dictionary<TKey, T>, Dictionary<TKey, T>>,
      DictionaryCollector<T, TKey, FuncSelector<T, TKey>, T, ItselfSelector<T>, DictionaryAddOrExceptionFunc<TKey, T>>
   > Dictionary<T, TKey> (this CollectorProvider<T> _, Func<T, TKey> keySelector, IEqualityComparer<TKey>? equaler = null) {
      return new DictionaryCollector<T, TKey, FuncSelector<T, TKey>, T, ItselfSelector<T>, DictionaryAddOrExceptionFunc<TKey, T>>(
         new FuncSelector<T, TKey>(keySelector),
         new ItselfSelector<T>(),
         new DictionaryAddOrExceptionFunc<TKey, T>(),
         equaler
      );
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Use<
      ICollector<T, Dictionary<TKey, TValue>, Dictionary<TKey, TValue>>,
      DictionaryCollector<T, TKey, FuncSelector<T, TKey>, TValue, FuncSelector<T, TValue>, TAddFunc>
   > Dictionary<T, TKey, TValue, TAddFunc> (
      this CollectorProvider<T> _,
      Func<T, TKey> keySelector,
      Func<T, TValue> valueSelector,
      ProvideDictionaryAddFunc<TKey, TValue, TAddFunc> provideAddFunc,
      IEqualityComparer<TKey>? equaler = null
   ) where TAddFunc: IDictionaryAddFunc<TKey, TValue> {
      return new DictionaryCollector<T, TKey, FuncSelector<T, TKey>, TValue, FuncSelector<T, TValue>, TAddFunc>(
         new FuncSelector<T, TKey>(keySelector),
         new FuncSelector<T, TValue>(valueSelector),
         provideAddFunc.Invoke(),
         equaler
      );
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Use<
      ICollector<T, Dictionary<TKey, TValue>, Dictionary<TKey, TValue>>,
      DictionaryCollector<T, TKey, FuncSelector<T, TKey>, TValue, FuncSelector<T, TValue>, DictionaryAddOrExceptionFunc<TKey, TValue>>
   > Dictionary<T, TKey, TValue> (
      this CollectorProvider<T> _,
      Func<T, TKey> keySelector,
      Func<T, TValue> valueSelector,
      IEqualityComparer<TKey>? equaler = null
   ) {
      return new DictionaryCollector<T, TKey, FuncSelector<T, TKey>, TValue, FuncSelector<T, TValue>, DictionaryAddOrExceptionFunc<TKey, TValue>>(
         new FuncSelector<T, TKey>(keySelector),
         new FuncSelector<T, TValue>(valueSelector),
         new DictionaryAddOrExceptionFunc<TKey, TValue>(),
         equaler
      );
   }
}
