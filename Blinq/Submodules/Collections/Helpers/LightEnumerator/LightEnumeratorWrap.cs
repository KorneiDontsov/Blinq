using System.Collections;

namespace Blinq.Collections;

sealed class LightEnumeratorWrap<T, TLightEnumerator>: IEnumerator<T> where TLightEnumerator: ILightEnumerator<T> {
   TLightEnumerator Enumerator;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public LightEnumeratorWrap (TLightEnumerator enumerator) {
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

static class LightEnumeratorWrap<T> {
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static LightEnumeratorWrap<T, TLightEnumerator> Create<TLightEnumerator> (TLightEnumerator enumerator)
   where TLightEnumerator: ILightEnumerator<T> {
      return new(enumerator);
   }
}
