namespace Turnify.Shared.Kernel.Common;

public static class NewUlid
{
    public static string Generate() => Ulid.NewUlid().ToString();
}
