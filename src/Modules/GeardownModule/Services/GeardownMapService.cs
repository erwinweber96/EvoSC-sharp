using EvoSC.Common.Interfaces.Models;
using EvoSC.Common.Interfaces.Services;
using EvoSC.Common.Models.Maps;
using EvoSC.Modules.Attributes;
using ManiaExchange.ApiClient;
using System.Globalization;

namespace EvoSC.Modules.Official.GeardownModule.Services;

public class GeardownMapService
{
    public async Task<MapMetadata?> getMap(int mxId, string? shortName)
    {
        var tmxApi = new MxTmApi("EvoSC#");

        var mapInfoDto = await tmxApi.GetMapInfoAsync(mxId, shortName);
        if (mapInfoDto == null)
        {
            return null;
        }

        var mapMetadata = new MapMetadata
        {
            MapUid = mapInfoDto.TrackUID,
            MapName = mapInfoDto.GbxMapName,
            AuthorId = mapInfoDto.AuthorLogin,
            AuthorName = mapInfoDto.Username,
            ExternalId = mapInfoDto.MapID.ToString(),
            ExternalVersion = Convert.ToDateTime(mapInfoDto.UpdatedAt, NumberFormatInfo.InvariantInfo).ToUniversalTime(),
            ExternalMapProvider = MapProviders.ManiaExchange
        };

        return mapMetadata;
    }

    public async Task<MapStream?> FindAndDownloadMapAsync(int mxId, string? shortName, IPlayer actor)
    {
        var tmxApi = new MxTmApi("EvoSC#");
        var mapFile = await tmxApi.DownloadMapAsync(mxId, shortName);

        if (mapFile == null)
        {
            return null;
        }

        var mapInfoDto = await tmxApi.GetMapInfoAsync(mxId, shortName);

        if (mapInfoDto == null)
        {
            return null;
        }

        var mapMetadata = new MapMetadata
        {
            MapUid = mapInfoDto.TrackUID,
            MapName = mapInfoDto.GbxMapName,
            AuthorId = mapInfoDto.AuthorLogin,
            AuthorName = mapInfoDto.Username,
            ExternalId = mapInfoDto.MapID.ToString(),
            ExternalVersion = Convert.ToDateTime(mapInfoDto.UpdatedAt, NumberFormatInfo.InvariantInfo).ToUniversalTime(),
            ExternalMapProvider = MapProviders.ManiaExchange
        };

        return new MapStream(mapMetadata, mapFile);
    }
}
