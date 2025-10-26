public class MovieFactory<T>
{
    private static T _currentKey;

    static MovieFactory()
    {
        _currentKey = default(T);
    }

    public static List<Movie<T>> GetMovieList()
    {
        var movieData = new List<Movie<T>>()
        {
            new Movie<T>
            {
                Key = KeyGeneratorReturnNext(),
                Title = "Lion King",
                Year = 1994,
                Category = "Animation, Family, Adventure",
                Description = "The Lion King (1994) is a classic Disney animated film that tells the story of a young lion named Simba who embarks on a journey to reclaim his throne as the king of the Pride Lands after the tragic death of his father."
            },
            new Movie<T>
            {
                Key = KeyGeneratorReturnNext(),
                Title = "Inception",
                Year = 2010,
                Category = "Science Fiction, Action, Thriller",
                Description = "Inception (2010) is a science fiction film directed by Christopher Nolan that follows a group of thieves who enter the dreams of their targets to steal information."
            },
            new Movie<T>
            {
                Key = KeyGeneratorReturnNext(),
                Title = "The Matrix",
                Year = 1999,
                Category = "Science Fiction, Action",
                Description = "The Matrix (1999) is a science fiction film directed by the Wachowskis that follows a computer hacker named Neo who discovers that the world he lives in is a simulated reality created by machines."
            },
            new Movie<T>
            {
                Key = KeyGeneratorReturnNext(),
                Title = "The Matrix Reloaded",
                Year = 2003,
                Category = "Science Fiction, Action",
                Description = "The Matrix Reloaded (2003), directed by the Wachowskis, is the second installment of the trilogy where Neo (Keanu Reeves), Trinity (Carrie-Anne Moss), and Morpheus (Laurence Fishburne) fight to protect Zion from the impending machine invasion, while Neo discovers deeper truths about the Matrix, faces the increasingly powerful Agent Smith (Hugo Weaving), and learns about his pivotal role from the enigmatic Architect."
            },
            new Movie<T>
            {
                Key = KeyGeneratorReturnNext(),
                Title = "The Matrix Revolutions",
                Year = 2003,
                Category = "Science Fiction, Action",
                Description = "The Matrix Revolutions (2003), directed by the Wachowskis, concludes the original trilogy with Neo (Keanu Reeves) facing his ultimate battle against the machines, alongside Trinity (Carrie-Anne Moss) and Morpheus (Laurence Fishburne), as Agent Smith (Hugo Weaving) threatens both the Matrix and the real world, culminating in a climactic sacrifice to bring balance and peace to humanity and machine-kind."
            },
            new Movie<T>
            {
                Key = KeyGeneratorReturnNext(),
                Title = "Shrek",
                Year = 2001,
                Category = "Animation, Comedy, Family",
                Description = "Shrek (2001), directed by Andrew Adamson and Vicky Jenson, is a groundbreaking animated comedy where an ogre named Shrek (voiced by Mike Myers) embarks on a quest to rescue Princess Fiona (voiced by Cameron Diaz) with the help of his talkative sidekick Donkey (voiced by Eddie Murphy), only to discover that true beauty lies within and happily-ever-after isn’t always what it seems."
            }
        };
        return movieData;
    }

    public static List<MovieVector<T>> GetMovieVectorList()
    {
        var movieData = GetMovieList();
        var movieVectorData = new List<MovieVector<T>>();
        foreach (var movie in movieData)
        {
            movieVectorData.Add(new MovieVector<T>
            {
                Key = movie.Key,
                Year = movie.Year,
                Category = movie.Category,
                Title = movie.Title,
                Description = movie.Description
            });
        }
        return movieVectorData;
    }

    public static List<MovieSQLite<T>> GetMovieSQLiteList()
    {
        var movieData = GetMovieList();
        var movieVectorData = new List<MovieSQLite<T>>();
        foreach (var movie in movieData)
        {
            movieVectorData.Add(new MovieSQLite<T>
            {
                Key = movie.Key,
                Title = movie.Title,
                Description = movie.Description
            });
        }
        return movieVectorData;
    }

    public static T KeyGeneratorReturnNext()
    {
        if (typeof(T) == typeof(int))
        {
            _currentKey = (T)Convert.ChangeType(Convert.ToInt32(_currentKey) + 1, typeof(T));
        }
        else if (typeof(T) == typeof(long))
        {
            _currentKey = (T)Convert.ChangeType(Convert.ToInt64(_currentKey) + 1, typeof(T));
        }
        else if (typeof(T) == typeof(ulong))
        {
            _currentKey = (T)Convert.ChangeType(Convert.ToUInt64(_currentKey) + 1, typeof(T));
        }
        else if (typeof(T) == typeof(string))
        {
            _currentKey = (T)Convert.ChangeType(Guid.NewGuid().ToString(), typeof(T));
        }
        else
        {
            throw new InvalidOperationException("Unsupported key type");
        }

        return _currentKey;
    }
}
