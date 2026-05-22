namespace Funeraria.Domain.Validators;

public static class DocumentoValidator
{
    public static string? NormalizarCpf(string? cpf) =>
        cpf is null ? null : new string(cpf.Where(char.IsDigit).ToArray());

    public static string? NormalizarCnpj(string? cnpj) =>
        cnpj is null ? null : new string(cnpj.Where(char.IsDigit).ToArray());

    public static bool IsCpfValido(string? cpf)
    {
        var d = NormalizarCpf(cpf);
        if (string.IsNullOrEmpty(d) || d.Length != 11) return false;
        if (d.Distinct().Count() == 1) return false;

        int[] mult1 = { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
        int[] mult2 = { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };

        var tempCpf = d[..9];
        var soma = 0;
        for (int i = 0; i < 9; i++) soma += (tempCpf[i] - '0') * mult1[i];
        var resto = soma % 11;
        var dig1 = resto < 2 ? 0 : 11 - resto;

        tempCpf += dig1;
        soma = 0;
        for (int i = 0; i < 10; i++) soma += (tempCpf[i] - '0') * mult2[i];
        resto = soma % 11;
        var dig2 = resto < 2 ? 0 : 11 - resto;

        return d.EndsWith($"{dig1}{dig2}");
    }

    public static bool IsCnpjValido(string? cnpj)
    {
        var d = NormalizarCnpj(cnpj);
        if (string.IsNullOrEmpty(d) || d.Length != 14) return false;
        if (d.Distinct().Count() == 1) return false;

        int[] mult1 = { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
        int[] mult2 = { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };

        var tempCnpj = d[..12];
        var soma = 0;
        for (int i = 0; i < 12; i++) soma += (tempCnpj[i] - '0') * mult1[i];
        var resto = soma % 11;
        var dig1 = resto < 2 ? 0 : 11 - resto;

        tempCnpj += dig1;
        soma = 0;
        for (int i = 0; i < 13; i++) soma += (tempCnpj[i] - '0') * mult2[i];
        resto = soma % 11;
        var dig2 = resto < 2 ? 0 : 11 - resto;

        return d.EndsWith($"{dig1}{dig2}");
    }
}
