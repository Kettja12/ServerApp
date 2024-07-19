public static partial class Extensions
{
    public static string Verify(this string? input,bool required=true)
    {
        if (required == false)
        {
            if (input==null) return "EmptyValueNotAllowed";
            if (input == "") return "EmptyValueNotAllowed";
        }
        if (input == null) return "";
        if (input.IndexOf("<")+ input.IndexOf(">")>0) return "Invalid";
        if (input.IndexOf("alter", StringComparison.OrdinalIgnoreCase) >= 0) return "InvalidValue"; 
        if (input.IndexOf("select", StringComparison.OrdinalIgnoreCase) >= 0) return "InvalidValue";
        if (input.IndexOf("insert", StringComparison.OrdinalIgnoreCase) >= 0) return "InvalidValue";
        if (input.IndexOf("delete", StringComparison.OrdinalIgnoreCase) >= 0) return "InvalidValue";
        if (input.IndexOf("drop", StringComparison.OrdinalIgnoreCase) >= 0) return "InvalidValue";
        return "";
    } 
}
