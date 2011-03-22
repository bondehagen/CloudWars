import java.util.ArrayList;

public class GameState {
    public ArrayList<Cloud> Thunderstorms = new ArrayList<Cloud>();
    public ArrayList<Cloud> Rainclouds = new ArrayList<Cloud>();
    public int MeIndex;

    public Cloud Me() {
        return Thunderstorms.get(MeIndex);
    }
}
