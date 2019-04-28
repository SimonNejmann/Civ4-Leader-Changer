using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Civ4_Leader_Changer
{
    // Each leader has a PlayerColor for their civilization.
    // Each PlayerColor has a major and a minor color (and a text color, but I ignore that here) - this is defined further down
    // PlayerColor is out here and public because this is the bit all the other parts need to see. Almost everything else here is private and hidden
    public enum PlayerColor
    {
        BLACK,
        BLACK_AND_GREEN,
        BLUE,
        LIGHT_BLUE,
        MIDDLE_BLUE,
        DARK_BLUE,
        BROWN,
        LIGHT_BROWN,
        CYAN,
        MIDDLE_CYAN,
        DARK_CYAN,
        CYAN_AND_GRAY,
        DARK_CYAN_AND_LEMON,
        GOLD_AND_BLACK,
        GRAY,
        DARK_GRAY,
        GREEN,
        LIGHT_GREEN,
        MIDDLE_GREEN,
        DARK_GREEN,
        DARK_DARK_GREEN,
        GREEN_AND_BLACK,
        GREEN_AND_WHITE,
        DARK_INDIGO,
        DARK_INDIGO_AND_WHITE,
        DARK_LEMON,
        ORANGE,
        LIGHT_ORANGE,
        DARK_ORANGE,
        ORANGE_AND_GREEN,
        PEACH,
        PINK,
        DARK_PINK,
        PURPLE,
        LIGHT_PURPLE,
        MIDDLE_PURPLE,
        DARK_PURPLE,
        RED,
        DARK_RED,
        RED_AND_GOLD,
        WHITE,
        YELLOW,
        LIGHT_YELLOW,
        DARK_YELLOW
    }

    static class PlayerColors
    {
        // Public methods:
        // Get the major (background) color from a PlayerColor
        // Returns a Brush that can be used to paint WPF elements
        public static Brush getBackground(PlayerColor pc)
        {
            var colors = playerColorDictionary[pc];
            var bg = gameColorDictionary[colors.Item1];
            return new SolidColorBrush(Color.FromScRgb(bg.a, bg.r, bg.g, bg.b));
        }

        // Get the minor (outline) color from a PlayerColor
        // Returns a Brush that can be used to paint WPF elements
        public static Brush getOutline(PlayerColor pc)
        {
            var colors = playerColorDictionary[pc];
            var ol = gameColorDictionary[colors.Item2];
            return new SolidColorBrush(Color.FromScRgb(ol.a, ol.r, ol.g, ol.b));
        }

        // Rest is private data.

        // As mentioned, each PlayerColor has a major and minor color - those two are GameColors.
        // These map to actual RGB values further down.
        private enum GameColor
        {
            COLOR_PLAYER_BLACK,
            COLOR_PLAYER_BLUE,
            COLOR_PLAYER_BROWN,
            COLOR_PLAYER_CYAN,
            COLOR_PLAYER_DARK_BLUE,
            COLOR_PLAYER_DARK_CYAN,
            COLOR_PLAYER_DARK_DARK_GREEN,
            COLOR_PLAYER_DARK_GRAY,
            COLOR_PLAYER_DARK_GREEN,
            COLOR_PLAYER_DARK_INDIGO,
            COLOR_PLAYER_DARK_LEMON,
            COLOR_PLAYER_DARK_ORANGE,
            COLOR_PLAYER_DARK_PINK,
            COLOR_PLAYER_DARK_PURPLE,
            COLOR_PLAYER_DARK_RED,
            COLOR_PLAYER_DARK_YELLOW,
            COLOR_PLAYER_GOLDENROD,
            COLOR_PLAYER_GRAY,
            COLOR_PLAYER_GREEN,
            COLOR_PLAYER_LIGHT_BLACK,
            COLOR_PLAYER_LIGHT_BLUE,
            COLOR_PLAYER_LIGHT_BROWN,
            COLOR_PLAYER_LIGHT_GREEN,
            COLOR_PLAYER_LIGHT_ORANGE,
            COLOR_PLAYER_LIGHT_PURPLE,
            COLOR_PLAYER_LIGHT_YELLOW,
            COLOR_PLAYER_MAROON,
            COLOR_PLAYER_MIDDLE_BLUE,
            COLOR_PLAYER_MIDDLE_CYAN,
            COLOR_PLAYER_MIDDLE_GREEN,
            COLOR_PLAYER_MIDDLE_PURPLE,
            COLOR_PLAYER_ORANGE,
            COLOR_PLAYER_PALE_ORANGE,
            COLOR_PLAYER_PALE_RED,
            COLOR_PLAYER_PEACH,
            COLOR_PLAYER_PINK,
            COLOR_PLAYER_PURPLE,
            COLOR_PLAYER_RED,
            COLOR_PLAYER_WHITE,
            COLOR_PLAYER_YELLOW,
            COLOR_PLAYER_DARK_RED_TEXT,
            COLOR_PLAYER_DARK_CYAN_TEXT,
            COLOR_PLAYER_CYAN_TEXT
        }

        // Struct for holding RGB+Alpha values - Prettier than using a Tuple<float,float,float,float> with unnamed (item1-4) fields.
        private struct ColorStruct
        {
            internal float r, g, b, a;
            // Constructor takes values as ARGB - this is because "Color.FromScRgb" goes in that order, and when I remembered that
            //  that didn't matter here, I had already populated the "gameColorDictionary" down below. By that time it was easier to change
            //  the order in the constructor than regex all those floats into a RGBA order... :)
            internal ColorStruct(float a, float r, float g, float b) { this.r = r; this.g = g; this.b = b; this.a = a; }
        }

        // Dictionary linking the PlayerColors to their major and minor GameColors
        private static readonly Dictionary<PlayerColor, Tuple<GameColor, GameColor>> playerColorDictionary = new Dictionary<PlayerColor, Tuple<GameColor, GameColor>>()
        {
            {PlayerColor.BLACK, Tuple.Create(GameColor.COLOR_PLAYER_BLACK, GameColor.COLOR_PLAYER_WHITE) },
            {PlayerColor.BLACK_AND_GREEN, Tuple.Create(GameColor.COLOR_PLAYER_LIGHT_BLACK, GameColor.COLOR_PLAYER_MIDDLE_GREEN) },
            {PlayerColor.BLUE, Tuple.Create(GameColor.COLOR_PLAYER_BLUE, GameColor.COLOR_PLAYER_WHITE) },
            {PlayerColor.BROWN, Tuple.Create(GameColor.COLOR_PLAYER_BROWN, GameColor.COLOR_PLAYER_DARK_YELLOW) },
            {PlayerColor.CYAN, Tuple.Create(GameColor.COLOR_PLAYER_CYAN, GameColor.COLOR_PLAYER_BLACK) },
            {PlayerColor.CYAN_AND_GRAY, Tuple.Create(GameColor.COLOR_PLAYER_CYAN, GameColor.COLOR_PLAYER_DARK_GRAY) },
            {PlayerColor.DARK_BLUE, Tuple.Create(GameColor.COLOR_PLAYER_DARK_BLUE, GameColor.COLOR_PLAYER_YELLOW) },
            {PlayerColor.DARK_CYAN, Tuple.Create(GameColor.COLOR_PLAYER_DARK_CYAN, GameColor.COLOR_PLAYER_WHITE) },
            {PlayerColor.DARK_CYAN_AND_LEMON, Tuple.Create(GameColor.COLOR_PLAYER_DARK_CYAN, GameColor.COLOR_PLAYER_LIGHT_YELLOW) },
            {PlayerColor.DARK_DARK_GREEN, Tuple.Create(GameColor.COLOR_PLAYER_DARK_DARK_GREEN, GameColor.COLOR_PLAYER_PALE_RED) },
            {PlayerColor.DARK_GRAY, Tuple.Create(GameColor.COLOR_PLAYER_DARK_GRAY, GameColor.COLOR_PLAYER_DARK_YELLOW) },
            {PlayerColor.DARK_GREEN, Tuple.Create(GameColor.COLOR_PLAYER_DARK_GREEN, GameColor.COLOR_PLAYER_YELLOW) },
            {PlayerColor.DARK_INDIGO, Tuple.Create(GameColor.COLOR_PLAYER_DARK_INDIGO, GameColor.COLOR_PLAYER_PALE_ORANGE) },
            {PlayerColor.DARK_INDIGO_AND_WHITE, Tuple.Create(GameColor.COLOR_PLAYER_DARK_INDIGO, GameColor.COLOR_PLAYER_WHITE) },
            {PlayerColor.DARK_LEMON, Tuple.Create(GameColor.COLOR_PLAYER_DARK_LEMON, GameColor.COLOR_PLAYER_BLACK) },
            {PlayerColor.DARK_ORANGE, Tuple.Create(GameColor.COLOR_PLAYER_DARK_ORANGE, GameColor.COLOR_PLAYER_BLACK) },
            {PlayerColor.DARK_PINK, Tuple.Create(GameColor.COLOR_PLAYER_DARK_PINK, GameColor.COLOR_PLAYER_YELLOW) },
            {PlayerColor.DARK_PURPLE, Tuple.Create(GameColor.COLOR_PLAYER_DARK_PURPLE, GameColor.COLOR_PLAYER_DARK_YELLOW) },
            {PlayerColor.DARK_RED, Tuple.Create(GameColor.COLOR_PLAYER_DARK_RED, GameColor.COLOR_PLAYER_DARK_YELLOW) },
            {PlayerColor.DARK_YELLOW, Tuple.Create(GameColor.COLOR_PLAYER_DARK_YELLOW, GameColor.COLOR_PLAYER_DARK_RED) },
            {PlayerColor.GOLD_AND_BLACK, Tuple.Create(GameColor.COLOR_PLAYER_GOLDENROD, GameColor.COLOR_PLAYER_BLACK) },
            {PlayerColor.GRAY, Tuple.Create(GameColor.COLOR_PLAYER_GRAY, GameColor.COLOR_PLAYER_BLACK) },
            {PlayerColor.GREEN, Tuple.Create(GameColor.COLOR_PLAYER_GREEN, GameColor.COLOR_PLAYER_BLACK) },
            {PlayerColor.GREEN_AND_BLACK, Tuple.Create(GameColor.COLOR_PLAYER_DARK_GREEN, GameColor.COLOR_PLAYER_BLACK) },
            {PlayerColor.GREEN_AND_WHITE, Tuple.Create(GameColor.COLOR_PLAYER_DARK_GREEN, GameColor.COLOR_PLAYER_WHITE) },
            {PlayerColor.LIGHT_BLUE, Tuple.Create(GameColor.COLOR_PLAYER_LIGHT_BLUE, GameColor.COLOR_PLAYER_BLACK) },
            {PlayerColor.LIGHT_BROWN, Tuple.Create(GameColor.COLOR_PLAYER_LIGHT_BROWN, GameColor.COLOR_PLAYER_BLACK) },
            {PlayerColor.LIGHT_GREEN, Tuple.Create(GameColor.COLOR_PLAYER_LIGHT_GREEN, GameColor.COLOR_PLAYER_DARK_BLUE) },
            {PlayerColor.LIGHT_ORANGE, Tuple.Create(GameColor.COLOR_PLAYER_LIGHT_ORANGE, GameColor.COLOR_PLAYER_DARK_DARK_GREEN) },
            {PlayerColor.LIGHT_PURPLE, Tuple.Create(GameColor.COLOR_PLAYER_LIGHT_PURPLE, GameColor.COLOR_PLAYER_BLACK) },
            {PlayerColor.LIGHT_YELLOW, Tuple.Create(GameColor.COLOR_PLAYER_LIGHT_YELLOW, GameColor.COLOR_PLAYER_BLACK) },
            {PlayerColor.MIDDLE_BLUE, Tuple.Create(GameColor.COLOR_PLAYER_MIDDLE_BLUE, GameColor.COLOR_PLAYER_DARK_RED_TEXT) },
            {PlayerColor.MIDDLE_CYAN, Tuple.Create(GameColor.COLOR_PLAYER_MIDDLE_CYAN, GameColor.COLOR_PLAYER_MAROON) },
            {PlayerColor.MIDDLE_GREEN, Tuple.Create(GameColor.COLOR_PLAYER_MIDDLE_GREEN, GameColor.COLOR_PLAYER_CYAN_TEXT) },
            {PlayerColor.MIDDLE_PURPLE, Tuple.Create(GameColor.COLOR_PLAYER_MIDDLE_PURPLE, GameColor.COLOR_PLAYER_GOLDENROD) },
            {PlayerColor.ORANGE, Tuple.Create(GameColor.COLOR_PLAYER_ORANGE, GameColor.COLOR_PLAYER_WHITE) },
            {PlayerColor.ORANGE_AND_GREEN, Tuple.Create(GameColor.COLOR_PLAYER_ORANGE, GameColor.COLOR_PLAYER_DARK_GREEN) },
            {PlayerColor.PEACH, Tuple.Create(GameColor.COLOR_PLAYER_PEACH, GameColor.COLOR_PLAYER_BLACK) },
            {PlayerColor.PINK, Tuple.Create(GameColor.COLOR_PLAYER_PINK, GameColor.COLOR_PLAYER_DARK_RED) },
            {PlayerColor.PURPLE, Tuple.Create(GameColor.COLOR_PLAYER_PURPLE, GameColor.COLOR_PLAYER_BLACK) },
            {PlayerColor.RED, Tuple.Create(GameColor.COLOR_PLAYER_RED, GameColor.COLOR_PLAYER_WHITE) },
            {PlayerColor.RED_AND_GOLD, Tuple.Create(GameColor.COLOR_PLAYER_RED, GameColor.COLOR_PLAYER_DARK_YELLOW) },
            {PlayerColor.WHITE, Tuple.Create(GameColor.COLOR_PLAYER_WHITE, GameColor.COLOR_PLAYER_RED) },
            {PlayerColor.YELLOW, Tuple.Create(GameColor.COLOR_PLAYER_YELLOW, GameColor.COLOR_PLAYER_DARK_BLUE) }
        };

        // Dictionary linking the GameColors to RGB values - exact values are taken from a Civ4 resource file.
        private static readonly Dictionary<GameColor, ColorStruct> gameColorDictionary = new Dictionary<GameColor, ColorStruct>()
        {
            {GameColor.COLOR_PLAYER_BLACK, new ColorStruct(1.00f, 0.13f, 0.13f, 0.13f) },
            {GameColor.COLOR_PLAYER_BLUE, new ColorStruct(1.00f, 0.21f, 0.40f, 1.00f) },
            {GameColor.COLOR_PLAYER_BROWN, new ColorStruct(1.00f, 0.39f, 0.24f, 0.00f) },
            {GameColor.COLOR_PLAYER_CYAN, new ColorStruct(1.00f, 0.07f, 0.80f, 0.96f) },
            {GameColor.COLOR_PLAYER_DARK_BLUE, new ColorStruct(1.00f, 0.16f, 0.00f, 0.64f) },
            {GameColor.COLOR_PLAYER_DARK_CYAN, new ColorStruct(1.00f, 0.00f, 0.54f, 0.55f) },
            {GameColor.COLOR_PLAYER_DARK_DARK_GREEN, new ColorStruct(1.00f, 0.00f, 0.27f, 0.00f) },
            {GameColor.COLOR_PLAYER_DARK_GRAY, new ColorStruct(1.00f, 0.369f, 0.369f, 0.369f) },
            {GameColor.COLOR_PLAYER_DARK_GREEN, new ColorStruct(1.00f, 0.00f, 0.39f, 0.00f) },
            {GameColor.COLOR_PLAYER_DARK_INDIGO, new ColorStruct(1.00f, 0.306f, 0.020f, 0.835f) },
            {GameColor.COLOR_PLAYER_DARK_LEMON, new ColorStruct(1.00f, 0.847f, 0.792f, 0.039f) },
            {GameColor.COLOR_PLAYER_DARK_ORANGE, new ColorStruct(1.00f, 0.878f, 0.235f, 0.000f) },
            {GameColor.COLOR_PLAYER_DARK_PINK, new ColorStruct(1.00f, 0.69f, 0.00f, 0.38f) },
            {GameColor.COLOR_PLAYER_DARK_PURPLE, new ColorStruct(1.00f, 0.45f, 0.00f, 0.49f) },
            {GameColor.COLOR_PLAYER_DARK_RED, new ColorStruct(1.00f, 0.62f, 0.00f, 0.00f) },
            {GameColor.COLOR_PLAYER_DARK_YELLOW, new ColorStruct(1.00f, 0.97f, 0.75f, 0.0f) },
            {GameColor.COLOR_PLAYER_GOLDENROD, new ColorStruct(1.00f, 0.871f, 0.624f, 0.000f) },
            {GameColor.COLOR_PLAYER_GRAY, new ColorStruct(1.00f, 0.7f, 0.7f, 0.7f) },
            {GameColor.COLOR_PLAYER_GREEN, new ColorStruct(1.00f, 0.49f, 0.88f, 0.00f) },
            {GameColor.COLOR_PLAYER_LIGHT_BLACK, new ColorStruct(1.00f, 0.251f, 0.251f, 0.251f) },
            {GameColor.COLOR_PLAYER_LIGHT_BLUE, new ColorStruct(1.00f, 0.50f, 0.70f, 1.00f) },
            {GameColor.COLOR_PLAYER_LIGHT_BROWN, new ColorStruct(1.00f, 0.518f, 0.345f, 0.075f) },
            {GameColor.COLOR_PLAYER_LIGHT_GREEN, new ColorStruct(1.00f, 0.50f, 1.00f, 0.50f) },
            {GameColor.COLOR_PLAYER_LIGHT_ORANGE, new ColorStruct(1.00f, 0.90f, 0.65f, 0.32f) },
            {GameColor.COLOR_PLAYER_LIGHT_PURPLE, new ColorStruct(1.00f, 0.70f, 0.60f, 1.00f) },
            {GameColor.COLOR_PLAYER_LIGHT_YELLOW, new ColorStruct(1.00f, 1.00f, 1.00f, 0.50f) },
            {GameColor.COLOR_PLAYER_MAROON, new ColorStruct(1.00f, 0.514f, 0.200f, 0.157f) },
            {GameColor.COLOR_PLAYER_MIDDLE_BLUE, new ColorStruct(1.00f, 0.000f, 0.220f, 0.914f) },
            {GameColor.COLOR_PLAYER_MIDDLE_CYAN, new ColorStruct(1.00f, 0.000f, 0.639f, 0.710f) },
            {GameColor.COLOR_PLAYER_MIDDLE_GREEN, new ColorStruct(1.00f, 0.204f, 0.576f, 0.000f) },
            {GameColor.COLOR_PLAYER_MIDDLE_PURPLE, new ColorStruct(1.00f, 0.675f, 0.118f, 0.725f) },
            {GameColor.COLOR_PLAYER_ORANGE, new ColorStruct(1.00f, 0.99f, 0.35f, 0.0f) },
            {GameColor.COLOR_PLAYER_PALE_ORANGE, new ColorStruct(1.00f, 0.863f, 0.471f, 0.149f) },
            {GameColor.COLOR_PLAYER_PALE_RED, new ColorStruct(1.00f, 0.780f, 0.282f, 0.239f) },
            {GameColor.COLOR_PLAYER_PEACH, new ColorStruct(1.00f, 0.60f, 0.49f, 0.0f) },
            {GameColor.COLOR_PLAYER_PINK, new ColorStruct(1.00f, 0.98f, 0.67f, 0.49f) },
            {GameColor.COLOR_PLAYER_PURPLE, new ColorStruct(1.00f, 0.77f, 0.34f, 1.00f) },
            {GameColor.COLOR_PLAYER_RED, new ColorStruct(1.00f, 0.86f, 0.02f, 0.02f) },
            {GameColor.COLOR_PLAYER_WHITE, new ColorStruct(1.00f, 0.90f, 0.90f, 0.90f) },
            {GameColor.COLOR_PLAYER_YELLOW, new ColorStruct(1.00f, 1.00f, 1.00f, 0.17f) },
            {GameColor.COLOR_PLAYER_DARK_RED_TEXT, new ColorStruct(1.00f, 1.00f, 0.22f, 0.22f) },
            {GameColor.COLOR_PLAYER_DARK_CYAN_TEXT, new ColorStruct(1.00f, 0f, .831f, .788f) },
            {GameColor.COLOR_PLAYER_CYAN_TEXT, new ColorStruct(1.00f, 0.6f, 1.0f, 0.973f) }
        };
    }
}
