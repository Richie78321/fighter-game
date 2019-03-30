using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FighterGame.Network.Client
{
    [Serializable]
    [ProtoContract(SkipConstructor = true)]
    public class LobbyInfo : IComparable<LobbyInfo>
    {
        //[ProtoMember(1)]
        //private LobbyAspect[] lobbyAspects;
        //public LobbyAspect[] LobbyAspects => lobbyAspects;

        [ProtoMember(1)]
        public readonly int TargetPlayers;

        public LobbyInfo(int targetPlayers, LobbyAspect[] lobbyAspects)
        {
            //Add max players lobby aspect
            //List<LobbyAspect> lobbyAspectList = new List<LobbyAspect>(lobbyAspects);
            //lobbyAspectList.Add(new MaxPlayers(maxPlayers));

            //Set lobby aspects
            //this.lobbyAspects = lobbyAspectList.ToArray();

            //Set var
            this.TargetPlayers = targetPlayers;
        }

        public int CompareTo(LobbyInfo lobbyInfo)
        {
            ////Check for lobby aspects
            //bool hasRequired = true;
            //int wantedScore = 0;
            //for (int i = 0; i < lobbyAspects.Length; i++)
            //{
            //    bool matched = false;
            //    for (int j = 0; j < lobbyInfo.LobbyAspects.Length; j++)
            //    {
            //        //Check for match
            //        if (lobbyInfo.LobbyAspects[j].CompareTo(lobbyAspects[i]) == 0)
            //        {
            //            //Match found
            //            matched = true;
            //            break;
            //        }
            //    }

            //    //Handle matched
            //    if (matched)
            //    {
            //        //Add to wanted score if a wanted aspect
            //        if (lobbyAspects[i].GetImportance() == LobbyAspect.Importance.Wanted) wantedScore++;
            //    }
            //    else if (lobbyAspects[i].GetImportance() == LobbyAspect.Importance.Required)
            //    {
            //        //Lobby does not have required aspect
            //        hasRequired = false;
            //        break;
            //    }
            //}

            ////Return score
            //if (hasRequired) return wantedScore;
            //else return -1;

            //TEMP
            if (TargetPlayers == lobbyInfo.TargetPlayers) return 0;
            else return -1;
        }
    }

    [Serializable]
    [ProtoContract(SkipConstructor = true)]
    public abstract class LobbyAspect : IComparable<LobbyAspect>
    {
        public enum Importance
        {
            Required,
            Wanted
        }

        //Object
        public abstract Importance GetImportance();

        public abstract int CompareTo(LobbyAspect lobbyAspect);
    }

    [Serializable]
    [ProtoContract(SkipConstructor = true)]
    public class MaxPlayers : LobbyAspect
    {
        [ProtoMember(1)]
        public readonly int MaxPlayerCount;

        public MaxPlayers(int maxPlayers)
        {
            MaxPlayerCount = maxPlayers;
        }

        public override Importance GetImportance() { return Importance.Required; }

        public override int CompareTo(LobbyAspect lobbyAspect)
        {
            //Ensure object is of same type
            if (lobbyAspect is MaxPlayers)
            {
                if (((MaxPlayers)lobbyAspect).MaxPlayerCount == MaxPlayerCount) return 0;
                else return -1;
            }
            else return -1;
        }
    }
}
