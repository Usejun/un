using Un.Object.Type;

namespace Un.Object.Function;

public class Future(Task<Obj> state) : Obj(UnType.Future)
{
    private Task<Obj> State { get; set; } = state;

    public void Run()
    {
        State.Start();
    }

    public Obj Wait() => State.Result;

    public override Future Clone() => new(State);
}