using System.Linq;

namespace Logic
{
    public class TestManagement
    {
        public int QuizScore { get; set; } = 0;

        public string[] UserAnswers { get; set; }

        public TestManagement(int i_NumOfQuestions)
        {
            UserAnswers = new string[i_NumOfQuestions];
        }

        public void CheckUserAnswers(UserFriendTestData i_TestData)
        {
            QuizScore = UserAnswers.Contains(i_TestData.City) ? QuizScore += 1 : QuizScore;
            QuizScore = UserAnswers.Contains(i_TestData.CurrentAge.ToString()) ? QuizScore += 1 : QuizScore;
            QuizScore = UserAnswers.Contains(i_TestData.BirthMonth.ToString()) ? QuizScore += 1 : QuizScore;
            QuizScore = UserAnswers.Contains(i_TestData.DayOfBirth) ? QuizScore += 1 : QuizScore;
        }

        public bool HasAnsweredAllQ()
        {
            bool answeredAll = true;

            for (int i = 0; i < UserAnswers.Length; i++) 
            {
                if (UserAnswers[i] == null)
                {
                    answeredAll = false;
                }
            }

            return answeredAll;
        }

        public void ResetTestData()
        {
            QuizScore = 0;
            for (int i = 0; i < UserAnswers.Length; i++) 
            {
                UserAnswers[i] = null;
            }
        }
    }
}