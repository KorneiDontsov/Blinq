namespace Blinq;

/// <inheritdoc />
/// <summary>A string iterator.</summary>
public struct StringIterator: IIterator<char> {
   readonly string Str;
   int Index;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   internal StringIterator (string str) {
      Str = str;
   }

   /// <inheritdoc />
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool TryPop (out char item) {
      if (Index < Str.Length) {
         item = Str[Index++];
         return true;
      } else {
         item = default;
         return false;
      }
   }

   /// <inheritdoc />
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public TAccumulator Fold<TAccumulator, TFold> (TAccumulator accumulator, TFold fold) where TFold: IFold<char, TAccumulator> {
      foreach (var item in Str.AsSpan(Index)) {
         ++Index;
         if (fold.Invoke(item, ref accumulator)) break;
      }

      return accumulator;
   }

   /// <inheritdoc />
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool TryGetCount (out int count) {
      count = Str.Length - Index;
      return true;
   }
}

public static partial class Iterator {
   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Contract<IIterator<char>, StringIterator> Iterate (this string str) {
      return new StringIterator(str);
   }
}
