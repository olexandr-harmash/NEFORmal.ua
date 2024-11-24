
using Microsoft.Net.Http.Headers;

namespace NEFORmal.ua.Dating.ApplicationCore.Models;

public class Profile
{
    public List<string> LastFiles { get; private set; }
    public int Id { get; private set; }

    private string _sid;
    public string Sid
    {
        get => _sid;
        private set
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentException("SID cannot be null or empty.", nameof(Sid));
            _sid = value;
        }
    }

    private string _name;
    public string Name
    {
        get => _name;
        private set
        {
            if (string.IsNullOrEmpty(value) || value.Length > 32)
                throw new ArgumentException("Name must be between 1 and 32 characters.", nameof(Name));
            _name = value;
        }
    }

    private string _bio;
    public string Bio
    {
        get => _bio;
        private set
        {
            if (value.Length > 255)
                throw new ArgumentException("Bio cannot be longer than 255 characters.", nameof(Bio));
            _bio = value;
        }
    }

    private int _age;
    public int Age
    {
        get => _age;
        private set
        {
            if (value < 0 || value > 120)
                throw new ArgumentException("Age must be between 0 and 120.", nameof(Age));
            _age = value;
        }
    }

    private string _sex;
    public string Sex
    {
        get => _sex;
        private set
        {
            if (string.IsNullOrEmpty(value) || (value != "Male" && value != "Female"))
                throw new ArgumentException("Sex must be either 'Male' or 'Female'.", nameof(Sex));
            _sex = value;
        }
    }

    private IEnumerable<string> _profilePhotos;

    public IEnumerable<string> ProfilePhotos
    {
        get => _profilePhotos;
        private set
        {
            if (value == null || !value.Any())
                throw new ArgumentException("Profile must have at least one photo.", nameof(ProfilePhotos));
            _profilePhotos = value;
        }
    }

    public Profile(string sid, string name, string bio, int age, string sex, List<string> fileNames)
    {
        Sid = sid;
        Name = name;
        Bio = bio;
        Age = age;
        Sex = sex;
        ProfilePhotos = fileNames;
    }

    public void UpdateAge(int newAge)
    {
        Age = newAge;
    }

    public void UpdateSex(string newSex)
    {
        Sex = newSex;
    }

    public void UpdateName(string newName)
    {
        Name = newName;
    }

    public void UpdateBio(string newBio)
    {
        Bio = newBio;
    }

    public void UpdateProfilePhotos(IEnumerable<string> profilePhotos)
    {
        LastFiles = ProfilePhotos.ToList();
        ProfilePhotos = profilePhotos;
    }

    public void UpdateProfile(string? name, string? bio, int? age, string? sex, List<string>? fileNames)
    {
        // Обновляем имя, если оно не null
        if (!string.IsNullOrEmpty(name))
        {
            UpdateName(name);
        }

        // Обновляем биографию, если она не null
        if (!string.IsNullOrEmpty(bio))
        {
            UpdateBio(bio);
        }

        // Обновляем возраст, если он не null
        if (age.HasValue)
        {
            UpdateAge(age.Value);
        }

        // Обновляем пол, если он не null
        if (!string.IsNullOrEmpty(sex))
        {
            UpdateSex(sex);
        }

        // Обновляем фотографии профиля, если они не null
        if (fileNames != null && fileNames.Any())
        {
            UpdateProfilePhotos(fileNames);
        }
    }
}
