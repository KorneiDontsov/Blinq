namespace Blinq;

public static class CollectPolicies {
   public sealed class AddOrSkip: IDictionaryCollectPolicy {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public static void Add<TKey, TValue> (Dictionary<TKey, TValue> dictionary, TKey key, TValue value) where TKey: notnull {
         _ = dictionary.TryAdd(key, value);
      }
   }

   public sealed class AddOrReplace: IDictionaryCollectPolicy {
      public static void Add<TKey, TValue> (Dictionary<TKey, TValue> dictionary, TKey key, TValue value) where TKey: notnull {
         dictionary[key] = value;
      }
   }

   public sealed class AddOrThrow: IDictionaryCollectPolicy {
      public static void Add<TKey, TValue> (Dictionary<TKey, TValue> dictionary, TKey key, TValue value) where TKey: notnull {
         dictionary.Add(key, value);
      }
   }
}

public static class CollectPolicy {
   [Pure] public static Type<CollectPolicies.AddOrSkip> AddOrSkip => default;
   [Pure] public static Type<CollectPolicies.AddOrReplace> AddOrReplace => default;
   [Pure] public static Type<CollectPolicies.AddOrThrow> AddOrThrow => default;
}
