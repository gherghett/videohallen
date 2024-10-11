using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace InputHandler;

public abstract class MenuItem
{
    public string Title {get; init;} = null!;
    protected Action? _onEnter = Console.WriteLine;
    protected MenuBuilder? _father = null;
    public abstract void Enter();

}

public class MenuBuilder : MenuItem
{
    private List<MenuItem> _children = new List<MenuItem>();
     
    private MenuBuilder(MenuBuilder? father, string title)
    {
        _father = father;
        Title = title;
    }
    public static MenuBuilder CreateMenu(string title) => 
        new MenuBuilder(null, title);

    public MenuBuilder AddMenu(string title)
    {
        var newMenu = new MenuBuilder(this, title);
        _children.Add(newMenu);
        return newMenu;
    }

    public MenuBuilder AddScreen(string title, Action action)
    {
        Screen newScreen = new Screen(this, title, action);
        _children.Add(newScreen);
        return this;
    }

    public MenuBuilder AddScreen(string title, List<Action> actions, bool goBack = true)
    {
        Screen newScreen = new Screen(this, title, actions, goBack);
        _children.Add(newScreen);
        return this;
    }

    public MenuBuilder Done() => 
        _father ?? throw new Exception("Cant go up this is root node");

    public MenuBuilder AddQuit(string title) =>
        this.AddScreen(title, [], false);
    
    public MenuBuilder AddQuit(string title, Action lastly) =>
        this.AddScreen(title, [lastly], false);

    public MenuBuilder OnEnter(Action onEnter){
        _onEnter = onEnter;
        return this;
    }

    public override void Enter()
    {
        _onEnter?.Invoke();

        var options = _children.Select( c => (c.Title, (Action)c.Enter)).ToList();
        if(_father is not null)
        {
            options.Insert(0,("Back: "+_father.Title, _father.Enter));
        }
        var choice = Chooser.ChooseAlternative(this.Title, options.ToArray());
        choice.Invoke();
    }
}

public class Screen : MenuItem
{
    private List<Action> _toDos = new List<Action>();
    private bool _goBack = true; 
    public Screen(MenuBuilder father, string title, Action action)
    {
        _father = father;
        Title = title;
        _toDos.Add(action);
    }

    public Screen(MenuBuilder father, string title, List<Action> actions, bool goBack = true)
    {
        _father = father;
        Title = title;
        _toDos.AddRange(actions);
        _goBack = goBack;
    }
    
    public override void Enter()
    {
        _onEnter?.Invoke();
        List<Action> toDos;
        if(_goBack)
        {
            toDos = _toDos
                .Append(_father!.Enter).ToList();
        }
        else
        {
            toDos = _toDos;
        }
        
        foreach(var todo in toDos)
        {
            todo.Invoke();
        }
    }
}