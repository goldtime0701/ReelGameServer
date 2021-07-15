using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReelServer
{
    public class CItemModel
    {
        public int m_nItemCode;
        public int m_nPrice;
        public int m_nGameCode;
        public string m_strItem;
        public string m_strNote;
        public int m_nLosePro;

        public CItemModel(int nItemCode)
        {
            m_nItemCode = nItemCode;
        }


        public const int ITEM_SWK_SONWUKONG = 0x01;
        public const int ITEM_SWK_JOPALGYE = 0x02;
        public const int ITEM_SWK_SAOJONG = 0x03;
        public const int ITEM_SWK_SAMJANG = 0x04;
        public const int ITEM_ALD_HORSE = 0x05;
        public const int ITEM_ALD_YANGTAN = 0x06;
        public const int ITEM_ALD_SMALL = 0x07;
        public const int ITEM_ALD_LARGE = 0x08;
        public const int ITEM_FDG_DRAGON = 0x09;
        public const int ITEM_SEA_JELLY = 0x0A;
        public const int ITEM_SEA_SHARK = 0x0B;
        public const int ITEM_SEA_WHALE = 0x0C;
        public const int ITEM_GDC_BUTTER = 0x0D;
        public const int ITEM_GDC_JOKER = 0x0E;
        public const int ITEM_OCA_SUBMARINE = 0x0F;
        public const int ITEM_OCA_BACKFISH = 0x10;
        public const int ITEM_OCA_ANIMATIC = 0x11;
        public const int ITEM_OCA_SHARK = 0x12;
        public const int ITEM_NWD_FBIRD = 0x13;
        public const int ITEM_NWD_GIRL = 0x14;
        public const int ITEM_YMT_YAMATO = 0x15;
        public const int ITEM_DVC_EARTH = 0x16;
        public const int ITEM_DVC_MONARIZA = 0x17;
        public const int ITEM_DVC_BAT = 0x18;
        public const int ITEM_WHT_OCTOR = 0x19;
        public const int ITEM_WHT_FISH = 0x1A;
        public const int ITEM_WHT_SHARK = 0x1B;
        public const int ITEM_WHT_WHALE = 0x1C;
        public const int ITEM_YAN_TURTLE = 0x1D;
        public const int ITEM_YAN_BDRAGON = 0x1E;
        public const int ITEM_YAN_GDRAGON = 0x1F;

    }

    public class CItemKeep
    {
        public int m_nItemModel;
        public int m_nItemCount;
        public int m_nGameCode;
    }
}
