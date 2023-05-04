namespace Blinq;

public static class CollectPolicies {
   public readonly struct AddOrSkip: IDictionaryCollectPolicy {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public static void Add<TKey, TValue> (Dictionary<TKey, TValue> dictionary, TKey key, TValue value) where TKey: notnull {
         _ = dictionary.TryAdd(key, value);
      }
   }

   public readonly struct AddOrReplace: IDictionaryCollectPolicy {
      public static void Add<TKey, TValue> (Dictionary<TKey, TValue> dictionary, TKey key, TValue value) where TKey: notnull {
         dictionary[key] = value;
      }
   }

   public readonly struct AddOrThrow: IDictionaryCollectPolicy {
      public static void Add<TKey, TValue> (Dictionary<TKey, TValue> dictionary, TKey key, TValue value) where TKey: notnull {
         dictionary.Add(key, value);
      }
   }
}

public static class CollectPolicy {
   public static Type<CollectPolicies.AddOrSkip> AddOrSkip => default;
   public static Type<CollectPolicies.AddOrReplace> AddOrReplace => default;
   public static Type<CollectPolicies.AddOrThrow> AddOrThrow => default;
}
