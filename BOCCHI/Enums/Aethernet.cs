using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using BOCCHI.Data;
using ECommons.DalamudServices;
using ECommons.GameHelpers;
using Lumina.Excel.Sheets;

namespace BOCCHI.Enums;

public enum Aethernet : uint
{
    BaseCamp = 4944,
    TheWanderersHaven = 4936,
    CrystallizedCaverns = 4929,
    Eldergrowth = 4930,
    Stonemarsh = 4942,
}

public class AethernetData
{
    public readonly static float DISTANCE = 4f;

    public Aethernet aethernet;

    public uint dataId;

    public Vector3 position;


    public Vector3 destination; // Where you end up after teleporting to this shard

    public static IEnumerable<AethernetData> All()
    {
        return ((Aethernet[])Enum.GetValues(typeof(Aethernet)))
            .Select(a => a.GetData());
    }

    public static IOrderedEnumerable<AethernetData> AllByDistance()
    {
        return AllByDistance(Player.Position);
    }

    public static IOrderedEnumerable<AethernetData> AllByDistance(Vector3 position)
    {
        return All().OrderBy(a => Vector3.Distance(a.position, position));
    }

    public static AethernetData GetClosestTo(Vector3 to)
    {
        return All().OrderBy(data => Vector3.Distance(to, data.position)).First();
    }

    public static AethernetData GetClosestToPlayer()
    {
        return GetClosestTo(Player.Position);
    }

    public float DistanceTo(Vector3 to)
    {
        return Vector3.Distance(to, position);
    }

    public float DistanceToPlayer()
    {
        return DistanceTo(Player.Position);
    }
}

public static class AethernetExtensions
{
    public static string ToFriendlyString(this Aethernet aethernet)
    {
        return Svc.Data.GetExcelSheet<PlaceName>().FirstOrDefault(p => p.RowId == (uint)aethernet).Name.ToString();
    }

    public static AethernetData GetData(this Aethernet aethernet)
    {
        switch (aethernet)
        {
            case Aethernet.BaseCamp:
                return new AethernetData
                {
                    aethernet = Aethernet.BaseCamp,
                    dataId = 2014664,
                    position = ZoneData.Aetherytes[ZoneData.SOUTHHORN],
                    destination = new Vector3(835.3f, 73f, -695.9f),
                };
            case Aethernet.TheWanderersHaven:
                return new AethernetData
                {
                    aethernet = Aethernet.TheWanderersHaven,
                    dataId = 2014665,
                    position = new Vector3(-173.02f, 8.19f, -611.14f),
                    destination = new Vector3(-169.1f, 6.5f, -609.4f),
                };
            case Aethernet.CrystallizedCaverns:
                return new AethernetData
                {
                    aethernet = Aethernet.CrystallizedCaverns,
                    dataId = 2014666,
                    position = new Vector3(-358.14f, 101.98f, -120.96f),
                    destination = new Vector3(-354.6f, 100f, -120.7f),
                };
            case Aethernet.Eldergrowth:
                return new AethernetData
                {
                    aethernet = Aethernet.Eldergrowth,
                    dataId = 2014667,
                    position = new Vector3(306.94f, 105.18f, 305.65f),
                    destination = new Vector3(-302.3f, 103f, 306f),
                };
            case Aethernet.Stonemarsh:
                return new AethernetData
                {
                    aethernet = Aethernet.Stonemarsh,
                    dataId = 2014744,
                    position = new Vector3(-384.12f, 99.20f, 281.42f),
                    destination = new Vector3(-384f, 97.2f, 278.1f),
                };
            default:
                return Aethernet.BaseCamp.GetData();
        }
    }
}
