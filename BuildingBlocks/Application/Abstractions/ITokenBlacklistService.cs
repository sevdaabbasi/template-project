namespace BuildingBlocks.Application.Abstractions;

public interface ITokenBlacklistService
{
    // Token'ı kalan süresi kadar kara listeye alır
    Task BlacklistTokenAsync(string token, TimeSpan timeToLive);
    
    // Token'ın kara listede olup olmadığını sorgular
    Task<bool> IsTokenBlacklistedAsync(string token);
}
