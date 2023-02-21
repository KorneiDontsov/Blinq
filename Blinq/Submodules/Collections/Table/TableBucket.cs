namespace Blinq.Collections;

readonly struct TableBucket {
   const int BucketAddendum = 1;

   readonly int Value;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   TableBucket (int value) {
      Value = value;
   }

   public static TableBucket Zero { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => new(BucketAddendum); }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static implicit operator TableBucket (int index) {
      return new(index + BucketAddendum);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static implicit operator int (TableBucket bucket) {
      return bucket.Value - BucketAddendum;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static implicit operator TableBucket (TableIndex index) {
      return (int)index;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static implicit operator TableIndex (TableBucket bucket) {
      return (int)bucket;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static TableBucket operator ++ (TableBucket bucket) {
      return new(bucket.Value + 1);
   }
}
