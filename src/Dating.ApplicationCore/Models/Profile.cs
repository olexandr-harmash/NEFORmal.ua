using Microsoft.Net.Http.Headers;

namespace NEFORmal.ua.Dating.ApplicationCore.Models;

/// <summary>
/// Represents a user profile in the dating application. It contains personal information, profile photos, and allows updating certain attributes.
/// </summary>
public class Profile
{
    /// <summary>
    /// Gets the collection of last uploaded files (e.g., profile photos).
    /// </summary>
    public IEnumerable<string> LastFiles { get; private set; }

    /// <summary>
    /// Gets the unique identifier for the profile.
    /// </summary>
    public int Id { get; private set; }

    // SID is a unique session identifier.
    private string _sid;

    /// <summary>
    /// Gets the session identifier for the profile.
    /// SID cannot be null or empty.
    /// </summary>
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

    // Name of the profile owner.
    private string _name;

    /// <summary>
    /// Gets the name of the profile owner. 
    /// Name must be between 1 and 32 characters.
    /// </summary>
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

    // Bio is a short description or personal statement of the user.
    private string _bio;

    /// <summary>
    /// Gets the bio of the profile owner.
    /// Bio cannot exceed 255 characters.
    /// </summary>
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

    // Age of the profile owner.
    private int _age;

    /// <summary>
    /// Gets the age of the profile owner. 
    /// Age must be between 0 and 120.
    /// </summary>
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

    // Sex/gender of the profile owner.
    private string _sex;

    /// <summary>
    /// Gets the sex of the profile owner. 
    /// Sex must be either 'Male' or 'Female'.
    /// </summary>
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

    // Profile photos of the user.
    private IEnumerable<string> _profilePhotos;

    /// <summary>
    /// Gets the collection of profile photos. 
    /// Profile must have at least one photo.
    /// </summary>
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

    /// <summary>
    /// Constructor to create a new profile with specified details.
    /// </summary>
    /// <param name="sid">Session identifier.</param>
    /// <param name="name">Profile owner’s name.</param>
    /// <param name="bio">Profile owner’s bio.</param>
    /// <param name="age">Profile owner’s age.</param>
    /// <param name="sex">Profile owner’s sex.</param>
    /// <param name="profilePhotos">Profile photos.</param>
    public Profile(string sid, string name, string bio, int age, string sex, IEnumerable<string> profilePhotos)
    {
        Sid = sid;
        Name = name;
        Bio = bio;
        Age = age;
        Sex = sex;
        LastFiles = ProfilePhotos;
        ProfilePhotos = profilePhotos;
    }

    /// <summary>
    /// Updates the age of the profile.
    /// </summary>
    /// <param name="newAge">The new age to set.</param>
    public void UpdateAge(int newAge)
    {
        Age = newAge;
    }

    /// <summary>
    /// Updates the sex (gender) of the profile.
    /// </summary>
    /// <param name="newSex">The new sex to set.</param>
    public void UpdateSex(string newSex)
    {
        Sex = newSex;
    }

    /// <summary>
    /// Updates the name of the profile.
    /// </summary>
    /// <param name="newName">The new name to set.</param>
    public void UpdateName(string newName)
    {
        Name = newName;
    }

    /// <summary>
    /// Updates the bio of the profile.
    /// </summary>
    /// <param name="newBio">The new bio to set.</param>
    public void UpdateBio(string newBio)
    {
        Bio = newBio;
    }

    /// <summary>
    /// Updates the profile photos. The previous photos will be stored in the LastFiles property.
    /// </summary>
    /// <param name="profilePhotos">The new set of profile photos.</param>
    public void UpdateProfilePhotos(IEnumerable<string> profilePhotos)
    {
        LastFiles = ProfilePhotos;
        ProfilePhotos = profilePhotos;
    }

    /// <summary>
    /// Updates the profile with optional new information (name, bio, age, sex, photos).
    /// </summary>
    /// <param name="name">New name, if provided.</param>
    /// <param name="bio">New bio, if provided.</param>
    /// <param name="age">New age, if provided.</param>
    /// <param name="sex">New sex, if provided.</param>
    /// <param name="fileNames">New profile photos, if provided.</param>
    public void UpdateProfile(string? name, string? bio, int? age, string? sex, List<string>? fileNames)
    {
        // Update name if not null or empty
        if (!string.IsNullOrEmpty(name))
        {
            UpdateName(name);
        }

        // Update bio if not null or empty
        if (!string.IsNullOrEmpty(bio))
        {
            UpdateBio(bio);
        }

        // Update age if it has a value
        if (age.HasValue)
        {
            UpdateAge(age.Value);
        }

        // Update sex if not null or empty
        if (!string.IsNullOrEmpty(sex))
        {
            UpdateSex(sex);
        }

        // Update profile photos if not null and contains photos
        if (fileNames != null && fileNames.Any())
        {
            UpdateProfilePhotos(fileNames);
        }
    }
}
