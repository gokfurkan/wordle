namespace Game.Dev.Scripts
{
    public enum SceneType
    {
        Load,
        Game,
    }

    public enum PanelType
    {
        Dev,
        OpenSettings,
        Settings, 
        Win, 
        Lose, 
        Level, 
        Money, 
        Restart,
        EndContinue,
        Board,
        Keyboard,
        GameName,
        Languages,
    }

    public enum LevelTextType
    {
        Level,
        LevelCompleted,
        LevelFailed,
    }

    public enum IncomeTextType
    {
        Win,
    }

    public enum CameraType
    {
        Menu,
    }

    public enum AudioType
    {
        GameStart,
    }

    public enum GameLanguage
    {
        English,
        Turkish,
    }
}