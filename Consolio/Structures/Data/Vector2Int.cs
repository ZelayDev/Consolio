using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Consolio
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct Vector2Int : IEquatable<Vector2Int>
    {
        #region Constants
        public static readonly Vector2Int Zero   = new(0, 0);
        public static readonly Vector2Int Left   = new(-1, 0);
        public static readonly Vector2Int Right  = new(1, 0);
        public static readonly Vector2Int Down   = new(0, -1);
        public static readonly Vector2Int Up     = new(0, 1);
        public static readonly Vector2Int OneOne = new(1);
        #endregion

        #region Data
        public int X, Y;
        #endregion

        #region Properties
        public float Magnitude => MathF.Sqrt(LengthSquared);

        public int LengthSquared => X * X + Y * Y;

        public Vector2Int Reversed => new(Y, X);

        public Vector2Int Normal
        {
            get
            {
                if (this == Zero)
                {
                    return Zero;
                }
                var Magnitude = this.Magnitude;
                return new Vector2Int((int)(X / Magnitude), (int)(Y / Magnitude));
            }
        }
        #endregion

        #region Constructors
        public Vector2Int(int Axes)
        {
            X = Axes;
            Y = Axes;
        }

        public Vector2Int(int X, int Y)
        {
            this.X = X;
            this.Y = Y;
        }

        private Vector2Int(SerializationInfo Info, StreamingContext Context)
        {
            X = Info.GetInt32("X");
            Y = Info.GetInt32("Y");
        }
        #endregion

        #region Operators
        #region MathOperators
        public static Vector2Int operator +(Vector2Int A, Vector2Int B)
        {
            return new Vector2Int(A.X + B.X, A.Y + B.Y);
        }

        public static Vector2Int operator -(Vector2Int A, Vector2Int B)
        {
            return new Vector2Int(A.X - B.X, A.Y - B.Y);
        }

        public static Vector2Int operator *(Vector2Int A, int B)
        {
            return new Vector2Int(A.X * B, A.Y * B);
        }

        public static Vector2Int operator /(Vector2Int A, int B)
        {
            return new Vector2Int(A.X / B, A.Y / B);
        }

        public static Vector2Int operator ++(Vector2Int Value)
        {
            Value.X++;
            Value.Y++;
            return Value;
        }

        public static Vector2Int operator --(Vector2Int Value)
        {
            Value.X--;
            Value.Y--;
            return Value;
        }

        public static Vector2Int operator -(Vector2Int Value)
        {
            return new Vector2Int(-Value.X, -Value.Y);
        }
        #endregion

        #region ComparisonOperators
        public static bool operator ==(Vector2Int A, Vector2Int B)
        {
            return A.X == B.X && A.Y == B.Y;
        }

        public static bool operator !=(Vector2Int A, Vector2Int B)
        {
            return A.X != B.X || A.Y != B.Y;
        }

        public static bool operator <(Vector2Int A, Vector2Int B)
        {
            return A.X < B.X && A.Y < B.Y;
        }

        public static bool operator >(Vector2Int A, Vector2Int B)
        {
            return A.X > B.X && A.Y > B.Y;
        }

        public static bool operator <=(Vector2Int A, Vector2Int B)
        {
            return A.X <= B.X && A.Y <= B.Y;
        }

        public static bool operator >=(Vector2Int A, Vector2Int B)
        {
            return A.X >= B.X && A.Y >= B.Y;
        }
        #endregion
        #endregion

        #region OverrideMethods
        public override string ToString()
        {
            return $"[{X}; {Y}]";
        }

        public override bool Equals([NotNullWhen(true)] object? Object)
        {
            return Object is Vector2Int Value && this == Value;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }
        #endregion

        #region IEquatable & IEqualityComparer
        public bool Equals(Vector2Int Other)
        {
            return this == Other;
        }

        public bool Equals(Vector2Int A, Vector2Int B)
        {
            return A == B;
        }

        public int GetHashCode(Vector2Int Object)
        {
            return Object.GetHashCode();
        }
        #endregion

        #region Methods
        #region Changins
        public void Normalize()
        {
            this = Normal;
        }
        #endregion

        #region Conversion
        public void Reverse()
        {
            this = Reversed;
        }
        #endregion

        #region Math
        public static Vector2Int Sum(params IEnumerable<Vector2Int> Values)
        {
            ArgumentNullException.ThrowIfNull(Values);

            var Result = Zero;

            foreach (var Value in Values)
            {
                Result += Value;
            }

            return Result;
        }


        public static Vector2Int Abs(in Vector2Int Value)
        {
            return new Vector2Int(Math.Abs(Value.X), Math.Abs(Value.Y));
        }

        public static Vector2Int Sign(in Vector2Int Value)
        {
            return new Vector2Int(int.Sign(Value.X), int.Sign(Value.Y));
        }


        public static Vector2Int Min(in Vector2Int A, in Vector2Int B)
        {
            return new Vector2Int(Math.Min(A.X, B.X), Math.Min(A.Y, B.Y));
        }

        public static Vector2Int Max(in Vector2Int A, in Vector2Int B)
        {
            return new Vector2Int(Math.Max(A.X, B.X), Math.Max(A.Y, B.Y));
        }

        public static Vector2Int Clamp(in Vector2Int Value, in Vector2Int Min, in Vector2Int Max)
        {
            return new Vector2Int
            (
                Math.Clamp(Value.X, Min.X, Max.X),
                Math.Clamp(Value.Y, Min.Y, Max.Y)
            );
        }


        public static int Dot(Vector2Int A, Vector2Int B)
        {
            return A.X * B.X + A.Y * B.Y;
        }


        public static float Distance(in Vector2Int A, in Vector2Int B)
        {
            return (A - B).Magnitude;
        }

        public static bool WithinRadius(in Vector2Int A, in Vector2Int B, float Radius)
        {
            return (A - B).LengthSquared <= Radius * Radius;
        }


        public static bool IsPositive(in Vector2Int Value)
        {
            return int.IsPositive(Value.X) && int.IsPositive(Value.Y);
        }

        public static bool IsNegative(in Vector2Int Value)
        {
            return int.IsNegative(Value.X) || int.IsNegative(Value.Y);
        }
        #endregion
        #endregion

        #region JsonConverter
        public sealed class JsonConverter : JsonConverter<Vector2Int>
        {
            public override void Write(Utf8JsonWriter Writer, Vector2Int Value, JsonSerializerOptions Options)
            {
                Writer.WriteStartArray();
                Writer.WriteNumberValue(Value.X);
                Writer.WriteNumberValue(Value.Y);
                Writer.WriteEndArray();
            }

            public override Vector2Int Read(ref Utf8JsonReader Reader, Type TypeToConvert, JsonSerializerOptions Options)
            {
                if (Reader.TokenType != JsonTokenType.StartArray)
                {
                    throw new JsonException("Expected an array [X, Y] for Vector2Int.");
                }

                Reader.Read();
                var X = Reader.GetInt32();

                Reader.Read();
                var Y = Reader.GetInt32();

                Reader.Read();
                if (Reader.TokenType != JsonTokenType.EndArray)
                {
                    throw new JsonException("Vector2Int array must contain exactly 2 numbers.");
                }

                return new Vector2Int(X, Y);
            }
        }
        #endregion
    }
}