namespace GenericVarianceContainerTests
{
    public interface A { }
    public interface B<in T> { }
    public interface C<in T1, in T2> { }
    public interface D<out T> { }
    public interface E<out T1, out T2> { }
    public interface F<in T1, out T2> { }

    public interface R { }
    public interface S { }
    public class T : R { }
    public class U : T { }
    public class V : S { }
    public class W : V { }


    public class AImpl : A { }
    public class BImpl : B<R> { }
    public class CImpl : C<R, S> { }
    public class DImpl : D<U> { }
    public class EImpl : E<U, W> { }
    public class FImpl : F<R, W> { }

    public abstract class CAbs<T1, T2> : C<R, S> { }
    public class CAbsImpl : CAbs<U, W> { }
}