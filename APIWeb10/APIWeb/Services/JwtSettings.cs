namespace APIWeb.Services;

public class JwtSettings
{
    public string Key { get; set; } = "CAMBIAR_ESTA_CLAVE_POR_UNA_MUY_LARGA_ALEATORIA_2026_NORTHWIND_API";
    public string Issuer { get; set; } = "APIWeb10";
    public string Audience { get; set; } = "APIWeb10Users";
    public int DurationInMinutes { get; set; } = 60;
}
