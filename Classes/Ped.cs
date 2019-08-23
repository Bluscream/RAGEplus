using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAGEplus
{
    public static class Ped
    {
        public static void setVariation(this Rage.Ped ped, Ped.Component component, int drawableIndex, Ped.TextureID texture) => ped.SetVariation((int)component, drawableIndex, (int)texture);
        /// <summary>
        /// void SET_PED_COMPONENT_VARIATION(Ped ped, int componentId, int drawableId, int textureId, int paletteId) 
        /// </summary>
        public enum Component {
            /// <summary>
            /// Texture Format head_diff_*
            /// </summary>
            Face = 0,
            /// <summary>
            /// Texture Format berd_diff_*
            /// </summary>
            Beard = 1,
            Haircut = 2,
            /// <summary>
            /// Texture Format uppr_diff_*
            /// </summary>
            Shirt = 3,
            /// <summary>
            /// Texture Format lowr_diff_*
            /// </summary>
            Pants = 4,
            Hands_gloves = 5,
            Shoes = 6,
            Eyes = 7,
            /// <summary>
            /// Texture Format accs_diff_*
            /// </summary>
            Accessories = 8,
            /// <summary>
            /// Texture Format task_diff_*
            /// </summary>
            Items_tasks = 9,
            /// <summary>
            /// Texture Format decl_diff_*
            /// </summary>
            Decals = 10,
            Collars_and_inner_shirts = 11
        }
        /// <summary>
        /// void SET_PED_PROP_INDEX(Ped ped, int componentId, int drawableId, int TextureId, int paletteId) 
        /// </summary>
        public enum Props {
            Head = 0,
            Eye = 1,
            Ear = 2
        }
        public enum TextureID {
            a,b,c,d,e,f,g,h,i,j,k,l,m,n,o,p,q,r,s,t,u,v,x,y,z
        }
    }
}
