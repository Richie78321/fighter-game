using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FighterGame.Entities
{
    [Serializable]
    [ProtoContract(SkipConstructor = true)]
    public struct SpriteManagerData
    {
        [ProtoMember(1)]
        public readonly int spriteState;
        [ProtoMember(2)]
        public readonly int spriteEvent;
        [ProtoMember(3)]
        public readonly int spriteSpecification;
        [ProtoMember(4)]
        public readonly SpriteManager.FacingStates facingStates;

        public SpriteManagerData(int spriteState, int spriteEvent, int spriteSpecification, SpriteManager.FacingStates facingState)
        {
            this.spriteState = spriteState;
            this.spriteEvent = spriteEvent;
            this.facingStates = facingState;
            this.spriteSpecification = spriteSpecification;
        }
    }
}
