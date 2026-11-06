namespace MeetupApi.Tests.Services;

/// <summary>
/// Relógio fixo para testes que dependem do horário exato (ex.: limite de 24h do cancelamento).
/// O fuso local é UTC para que GetLocalNow() devolva exatamente o horário informado.
/// </summary>
public class FixedTimeProvider : TimeProvider
{
    private readonly DateTimeOffset _now;

    public FixedTimeProvider(DateTime now)
    {
        _now = new DateTimeOffset(now, TimeSpan.Zero);
    }

    public override DateTimeOffset GetUtcNow() => _now;

    public override TimeZoneInfo LocalTimeZone => TimeZoneInfo.Utc;
}
