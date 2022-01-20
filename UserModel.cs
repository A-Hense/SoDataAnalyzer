namespace SoDataAnalyzer
{
    class UserModel    
    {
        public UserModel (int user){
            userId=user;
            numberOfAnswers=0;
            numberOfNegativeScores=0;
            numberOfPositiveScores=0;
            numberOfQuestions=0;
            numberOfTotalScores=0;
            activityPeriod="";
        }
        public int userId{set;get;}
        public int numberOfQuestions{set;get;}
        public int numberOfAnswers{set;get;}
        public int numberOfPositiveScores{set;get;}
        public int numberOfNegativeScores{set;get;}
        public int numberOfTotalScores{set;get;}
        public string activityPeriod{set;get;}
    }
}