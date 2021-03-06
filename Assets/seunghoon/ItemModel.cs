using System;

namespace seunghoon
{
    [Serializable]
    public class ItemModel
    {
        public string itemName;
        public string player;
        ItemModel(string itemName, string player)
        {
            this.itemName = itemName;
            this.player = player;
        }

        public static string ParseItemToString(ItemNameEnum itemNameEnum)
        {
            switch (itemNameEnum)
            {
                case ItemNameEnum.DoubleScore:
                    return "DoubleScore";
                case ItemNameEnum.BigGoal:
                    return "BigGoal";
                case ItemNameEnum.SmallGoal:
                    return "SmallGoal";
                case ItemNameEnum.PenaltyKick:
                    return "PenaltyKick";
                default:
                    return "";
            }
        }

        public static ItemNameEnum ParseStringToItemNameEnum(string str)
        {
            switch (str)
            {
                case "DoubleScore":
                    return ItemNameEnum.DoubleScore;
                case "BigGoal" :
                    return ItemNameEnum.BigGoal;
                case "SmallGoal" :
                    return ItemNameEnum.SmallGoal;
                case "PenaltyKick":
                    return ItemNameEnum.PenaltyKick;
                default:
                    return ItemNameEnum.None;
            }
        }
    }

    public enum ItemNameEnum
    {
        None, DoubleScore, BigGoal, SmallGoal, PenaltyKick
    }
}