import java.io.IOException;

public class MyAi extends Client {

    public MyAi() throws IOException, InterruptedException {
        super();
    }

    // Implement your AI here
    // Use these functions to communicate with server:
    //    SetName(name) - Sets your name to appear in the simulator
    //    GetState() - returns the current GameState object
    //    Wind(x, y) - applies a wind in the given direction (returns true if OK, false if IGNORED)
    @Override
    public void RunAi() throws IOException, InterruptedException {
        SetName("StupidAI");

        while (Connected()) {
            // Poll the game state
            GameState state = GetState();
            if (state == null) break;

            // Ignore game state and do something random!
            Wind((float) Math.random() * 25 - 50, (float) Math.random() * 25 - 50);

            Thread.sleep(500);
        }
    }

    public static void main(String[] args) {
        try {
            MyAi ai = new MyAi();
        } catch (Exception e) {
            e.printStackTrace();
        }
    }

}


