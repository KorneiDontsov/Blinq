namespace Blinq;

static class StructUtils {
   public static T Exchange<T> (this ref T location, T value) where T: struct {
      var result = location;
      location = value;
      return result;
   }
}
