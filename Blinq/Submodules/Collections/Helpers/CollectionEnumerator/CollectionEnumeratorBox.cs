using System.Collections;

namespace Blinq.Collections;

sealed class CollectionEnumeratorBox<T, TLightEnumerator>: IEnumerator<T> where TLightEnumerator: ICollectionEnumerator<T> {
   TLightEnumerator Enumerator;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public CollectionEnumeratorBox (TLightEnumerator enumerator) {
      Enumerator = enumerator;
   }

   public T Current => Enumerator.Current;

   object IEnumerator.Current => Current!;

   public bool MoveNext () {
      return Enumerator.MoveNext();
   }

   public void Reset () {
      Get.Throw<NotSupportedException>();
   }

   public void Dispose () { }
}

static class CollectionEnumeratorBox<T> {
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static CollectionEnumeratorBox<T, TLightEnumerator> Create<TLightEnumerator> (TLightEnumerator enumerator)
   where TLightEnumerator: ICollectionEnumerator<T> {
      return new(enumerator);
   }
}
