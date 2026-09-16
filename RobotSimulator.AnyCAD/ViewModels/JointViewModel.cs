using RobotSimulator.Models;

namespace RobotSimulator.ViewModels;

public sealed class JointViewModel : ObservableObject
{
    private double _angle;
    private double _minimum;
    private double _maximum;
    private double _homeAngle;
    private double _modelReferenceAngle;
    private double _axisOriginX;
    private double _axisOriginY;
    private double _axisOriginZ;
    private double _axisDirectionX;
    private double _axisDirectionY;
    private double _axisDirectionZ;

    public JointViewModel(int index, JointConfiguration configuration)
    {
        Index = index;
        Name = configuration.Name;
        ModelFile = configuration.ModelFile;
        _minimum = configuration.Minimum;
        _maximum = configuration.Maximum;
        _homeAngle = configuration.HomeAngle;
        _modelReferenceAngle = configuration.ModelReferenceAngle;
        _angle = configuration.HomeAngle;
        _axisOriginX = configuration.AxisOrigin.X;
        _axisOriginY = configuration.AxisOrigin.Y;
        _axisOriginZ = configuration.AxisOrigin.Z;
        _axisDirectionX = configuration.AxisDirection.X;
        _axisDirectionY = configuration.AxisDirection.Y;
        _axisDirectionZ = configuration.AxisDirection.Z;
    }

    public event EventHandler? StateChanged;

    public int Index { get; }

    public string Name { get; }

    public string ControllerAxis => $"A{Index + 1}";

    public string DisplayName => $"{ControllerAxis} / {Name}";

    public string ModelFile { get; }

    public double Angle
    {
        get => _angle;
        set
        {
            var clamped = Math.Clamp(value, Minimum, Maximum);
            if (SetProperty(ref _angle, clamped))
            {
                StateChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    public double Minimum
    {
        get => _minimum;
        set
        {
            if (value >= Maximum || !SetProperty(ref _minimum, value))
            {
                return;
            }

            Angle = Angle;
            StateChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public double Maximum
    {
        get => _maximum;
        set
        {
            if (value <= Minimum || !SetProperty(ref _maximum, value))
            {
                return;
            }

            Angle = Angle;
            StateChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public double HomeAngle
    {
        get => _homeAngle;
        set => SetKinematicProperty(ref _homeAngle, value);
    }

    public double ModelReferenceAngle
    {
        get => _modelReferenceAngle;
        set => SetKinematicProperty(ref _modelReferenceAngle, value);
    }

    public double AxisOriginX
    {
        get => _axisOriginX;
        set => SetKinematicProperty(ref _axisOriginX, value);
    }

    public double AxisOriginY
    {
        get => _axisOriginY;
        set => SetKinematicProperty(ref _axisOriginY, value);
    }

    public double AxisOriginZ
    {
        get => _axisOriginZ;
        set => SetKinematicProperty(ref _axisOriginZ, value);
    }

    public double AxisDirectionX
    {
        get => _axisDirectionX;
        set => SetKinematicProperty(ref _axisDirectionX, value);
    }

    public double AxisDirectionY
    {
        get => _axisDirectionY;
        set => SetKinematicProperty(ref _axisDirectionY, value);
    }

    public double AxisDirectionZ
    {
        get => _axisDirectionZ;
        set => SetKinematicProperty(ref _axisDirectionZ, value);
    }

    public JointConfiguration ToConfiguration()
    {
        return new JointConfiguration
        {
            Name = Name,
            ModelFile = ModelFile,
            Minimum = Minimum,
            Maximum = Maximum,
            HomeAngle = HomeAngle,
            ModelReferenceAngle = ModelReferenceAngle,
            AxisOrigin = new VectorConfiguration
            {
                X = AxisOriginX,
                Y = AxisOriginY,
                Z = AxisOriginZ
            },
            AxisDirection = new VectorConfiguration
            {
                X = AxisDirectionX,
                Y = AxisDirectionY,
                Z = AxisDirectionZ
            }
        };
    }

    private void SetKinematicProperty(ref double field, double value, [System.Runtime.CompilerServices.CallerMemberName] string? propertyName = null)
    {
        if (SetProperty(ref field, value, propertyName))
        {
            StateChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
