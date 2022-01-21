
using System.Text.RegularExpressions;

namespace SoDataAnalyzer
{
    class Program
    {
        static void Main(string[] args)
        {
            //slimDatasetAnswers();
            //slimDatasetQuestions();
            //generateEnrichedUserdata();
            analyzeUserContributions();
            //analyzeUserGrowth();
            //analyzeUserLoss();
            //createQuestionDictionary();
            //scoreRatioForYears();
        }

        static private void slimDatasetAnswers()
        {
            string[] lines = System.IO.File.ReadAllLines("Answers.csv");
            List<String> mylist = new List<String>();
            int i=0;
            foreach(var line in lines)
            {
                if(i==0)
                {
                    mylist.Add("Id,OwnerUserId,CreationDate,ParentId,Score");
                    i++;
                    continue;
                }
                String[] splittedLine = line.Split(",");
                if(splittedLine.Length > 4 && !splittedLine[0].Contains("<"))
                {
                    int test=0;
                    if(Int32.TryParse(splittedLine[0],out test) && test > 469)
                    {
                        String newline="";
                        if(splittedLine[4].Contains("\""))
                        {
                            for(int k=0; k<splittedLine.Length;k++)
                            {
                                if(splittedLine[k].Contains("<"))
                                {
                                    continue;
                                }
                                newline+=splittedLine[k]+",";
                            }
                        } 
                        else
                        {
                             newline=splittedLine[0]+","+splittedLine[1]+","+splittedLine[2]+","+splittedLine[3]+","+splittedLine[4];
                        }
                        mylist.Add(newline);            
                    }  
                }
                i++;
            } 
            File.WriteAllLines("Answers_ohneBody.csv", mylist.ToArray());
            Console.WriteLine("Answers slimmed down");
        }

        static private void slimDatasetQuestions()
        {
            string[] lines = System.IO.File.ReadAllLines("Questions.csv");
            List<String> mylist = new List<String>();
            int i=0;
            int previousDataId=468;
            foreach(var line in lines)
            {
                if(i==0)
                {
                    mylist.Add("Id,OwnerUserId,CreationDate,Score,Title");
                    i++;
                    continue;
                }
                String[] splittedLine = line.Split(",");
                if(splittedLine.Length > 4 && !splittedLine[0].Contains("<"))
                {
                    int reasonableID=0;
                    int reasonableYear=0;
                    int reasonableuser=0;
                    if(Int32.TryParse(splittedLine[0],out reasonableID) 
                    && splittedLine[2].Length >15
                    && Int32.TryParse(splittedLine[2].Substring(0,4),out reasonableYear) 
                    && Int32.TryParse(splittedLine[1],out reasonableuser) 
                    && reasonableID > previousDataId 
                    && reasonableYear > 2006
                    && reasonableYear < 2022)
                    {
                        String newline="";
                        if(splittedLine[4].Contains("\""))
                        {
                            for(int k=0; k<splittedLine.Length;k++)
                            {
                                if(splittedLine[k].Contains("<"))
                                {
                                    continue;
                                }
                                newline+=splittedLine[k]+",";
                            }
                        } 
                        else
                        {
                             newline=splittedLine[0]+","+splittedLine[1]+","+splittedLine[2]+","+splittedLine[3]+","+splittedLine[4];
                        }
                        mylist.Add(newline);    
                        previousDataId=Int32.Parse(splittedLine[0]);        
                    } 
                }
                i++;
            } 
            File.WriteAllLines("Questions_ohneBody.csv", mylist.ToArray());
            Console.WriteLine("Questions slimmed down.");
        }
        static private void generateEnrichedUserdata()
        {
            List<UserModel> analyzedUsers = new List<UserModel>();
            analyzeQuestions(analyzedUsers);
            analyzeAnswers(analyzedUsers);
            List<string> fileLines = new List<string>();
            fileLines.Add("UserId;NumberOfAnswers;NumberOfNegativScores;NumberOfPositiveScores;NumberOfQuestions;NumberOfTotalScores;ActivityPeriod");
            foreach(UserModel user in analyzedUsers){
                fileLines.Add(user.userId+";"+user.numberOfAnswers+";"+user.numberOfNegativeScores+";"+user.numberOfPositiveScores+";"+user.numberOfQuestions+";"+user.numberOfTotalScores+";"+user.activityPeriod.Substring(0,user.activityPeriod.Length-1));
            }
            File.WriteAllLines("analyzedUsers.csv", fileLines.ToArray());
            Console.WriteLine("users analyzed");
        }
        static public void analyzeUserContributions()
        {
            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"analyzedUsers.csv");
            string[] lines = System.IO.File.ReadAllLines(path);
            List<String> mylist = new List<String>();
            int i=0;
            int[] years = new int[9];
            foreach(var line in lines)
            {
                if(i==0)
                {
                    i++;
                    continue;
                }
                string[] lineValues = line.Split(";");
                string[] yearValues = lineValues[6].Split(",");
                foreach(var year1 in yearValues)
                {
                    switch (year1)
                    {
                        case "2008":
                        years[0]++;
                        break;
                        case "2009":
                        years[1]++;
                        break;
                        case "2010":
                        years[2]++;
                        break;
                        case "2011":
                        years[3]++;
                        break;
                        case "2012":
                        years[4]++;
                        break;
                        case "2013":
                        years[5]++;
                        break;
                        case "2014":
                        years[6]++;
                        break;
                        case "2015":
                        years[7]++;
                        break;
                        case "2016":
                        years[8]++;
                        break;
                        default:
                        break;
                    }
                    
                }
            }
            int year=2008;
            int total =0;
            foreach(var yearCount in years)
            {
                total+=yearCount;
                Console.WriteLine("Beiträge in "+year+": "+yearCount+"  Gesamt: "+total);
                year++;
            }
        }
        static public void createQuestionDictionary()
        {
            string[] lines = System.IO.File.ReadAllLines("Questions_ohneBody.csv");
            Dictionary<string, int> wordcounter = new Dictionary<string, int>();
            int i=0;
            foreach(var line in lines)
            {
                if(i==0)
                {
                    i++;
                    continue;
                }
                string[] lineColumns = line.Split(",");
                string[] titleWords = lineColumns[4].Split(" ");
                foreach(var word in titleWords)
                {
                    string tranformedWord="";
                    Regex rgx = new Regex("[^a-zA-Z0-9]");
                    tranformedWord = rgx.Replace(word, "");
                    tranformedWord = tranformedWord.ToLower();
                    if(!wordcounter.ContainsKey(tranformedWord))
                    {
                        wordcounter.Add(tranformedWord,1);
                    } else
                    {
                        wordcounter[tranformedWord]+=1;
                    }
                }
            }
            List<KeyValuePair<string, int>> myList = wordcounter.ToList();
            myList.Sort(
                delegate(KeyValuePair<string, int> pair1,
                KeyValuePair<string, int> pair2)
                {
                    return pair1.Value.CompareTo(pair2.Value);
                }
            );
            foreach(var item in myList)
            {
                if(item.Value>2){
                    Console.WriteLine("Wort:'"+item.Key+"'  Anzahl an Treffern: "+item.Value.ToString());
                }
            }
            Console.WriteLine("Wörter insgesamt: "+wordcounter.Count);
        }    
        static public void scoreRatioForYears()
        {
            string[] lines = System.IO.File.ReadAllLines("Questions_ohneBody.csv");
            Dictionary<string, int> wordcounter = new Dictionary<string, int>();
            int i=0;
            int[] scoreCountForYears = new int[18];
            foreach(var line in lines)
            {
                if(i==0)
                {
                    i++;
                    continue;
                }
                string[] lineColumns = line.Split(",");
                if(Int32.Parse(lineColumns[3])>=0)
                {
                    scoreCountForYears[Int32.Parse(lineColumns[2].Substring(0,4))-2008]+=1;
                } 
                else
                {
                    scoreCountForYears[Int32.Parse(lineColumns[2].Substring(0,4))-1999]+=1;
                }
            }
            Console.WriteLine("      Positive   |   Negative   |   % positiv");
            for(int j=0;j<9;j++)
            {
                Console.WriteLine(""+(2008+j)+":     "+scoreCountForYears[j]+"         "+scoreCountForYears[j+9]+"         "+((float)(scoreCountForYears[j]) / ((float)(scoreCountForYears[j+9]+scoreCountForYears[j])))+"%");
            }
        }

        static private void analyzeUserGrowth()

        {
            string[] lines = System.IO.File.ReadAllLines("analyzedUsers.csv");
            List<String> mylist = new List<String>();
            int i=0;
            int[] years = new int[9];
            int[] yearsloss = new int[9];
            foreach(var line in lines)
            {
                if(i==0)
                {
                    i++;
                    continue;
                }
                string[] lineValues = line.Split(";");
                string[] yearValues = lineValues[6].Split(",");
                List<int> yearValuesInt = new List<int>();
                foreach(string year1 in yearValues){
                    int rightfullyParsed;
                    if(Int32.TryParse(year1,out rightfullyParsed) && rightfullyParsed>2007 && rightfullyParsed <2017)
                    {
                        yearValuesInt.Add(rightfullyParsed);
                    }  
                }
                if(yearValuesInt.Count>0)
                {
                    years[2016-yearValuesInt.Min()]++;
                    if(yearValuesInt.Max()<2016){
                        yearsloss[2016-yearValuesInt.Max()-1]--;
                    }
                    
                } 
            }
            int year=2008;
            int total =0;
            for(int j=years.Length-1;j>=0;j--)
            {
                total+=years[j]+yearsloss[j];
                Console.WriteLine("Neue Nutzer in "+year+": "+years[j]+" Nutzer verloren: "+yearsloss[j]+"  Gesamtnutzer \"aktiv\": "+total);
                year++;
            }
        } 
        static private void analyzeUserLoss()
        
        {
            string[] lines = System.IO.File.ReadAllLines("analyzedUsers.csv");
            List<String> mylist = new List<String>();
            int i=0;
            int[] years = new int[9];
            int averageactivity=0;
            int usercount=0;
            foreach(var line in lines)
            {
                if(i==0)
                {
                    i++;
                    continue;
                }
                string[] lineValues = line.Split(";");
                string[] yearValues = lineValues[6].Split(",");
                List<int> yearValuesInt = new List<int>();
                foreach(string year1 in yearValues){
                    int rightfullyParsed;
                    if(Int32.TryParse(year1,out rightfullyParsed) && rightfullyParsed>2007 && rightfullyParsed <2017)
                    {
                        yearValuesInt.Add(rightfullyParsed);
                    }  
                }
                if(yearValuesInt.Count>0)
                {
                    averageactivity+= yearValuesInt.Max()-yearValuesInt.Min()+1;
                    usercount++;
                } 
            }
            Console.WriteLine("Nutzeraktivität im Schnitt: "+((double)averageactivity)/((double)usercount)+" Jahre.");
        } 
        static private void analyzeQuestions( List<UserModel> analyzedUsers)
        {
            string[] lines = System.IO.File.ReadAllLines("Questions_ohneBody.csv");
            int i=0;
            foreach(var line in lines)
            {
                if(i==0){
                    i++;
                    continue;
                }
                string[] lineColumns =line.Split(",");
                int userId =0;
                int year=0;
                int score=0;
                try{
                    userId = Int32.Parse(lineColumns[1]);
                    year = Int32.Parse(lineColumns[2].Substring(0,4));
                    score = Int32.Parse(lineColumns[3]);
                } catch(Exception e)
                {
                    continue;
                }
                UserModel currentUser = analyzedUsers.Find(user => user.userId == userId);
                if(currentUser == null)
                {
                   currentUser = new UserModel(userId);
                   analyzedUsers.Add(currentUser);
                }
                if(score >= 0)
                {
                    currentUser.numberOfPositiveScores++;
                }
                else 
                {
                    currentUser.numberOfNegativeScores++;
                }
                currentUser.numberOfTotalScores=currentUser.numberOfTotalScores + score;
                currentUser.activityPeriod+=year+",";
                currentUser.numberOfQuestions++;
                i++;
            }
        }
        static private void analyzeAnswers(List<UserModel> analyzedUsers)
        {
            string[] lines = System.IO.File.ReadAllLines("Answers_ohneBody.csv");
            int i=0;
            foreach(var line in lines)
            {
                if(i==0){
                    i++;
                    continue;
                }
                string[] lineColumns =line.Split(",");
                int userId =0;
                int year=0;
                int score=0;
                try{
                    userId = Int32.Parse(lineColumns[1]);
                    year = Int32.Parse(lineColumns[2].Substring(0,4));
                    score = Int32.Parse(lineColumns[4]);
                } catch(Exception e)
                {
                    continue;
                }
                UserModel currentUser = analyzedUsers.Find(user => user.userId == userId);
                if(currentUser == null)
                {
                   currentUser = new UserModel(userId);
                   analyzedUsers.Add(currentUser);
                }
                if(score >= 0)
                {
                    currentUser.numberOfPositiveScores++;
                }
                else 
                {
                    currentUser.numberOfNegativeScores++;
                }
                currentUser.numberOfTotalScores=currentUser.numberOfTotalScores + score;
                currentUser.activityPeriod+=year+",";
                currentUser.numberOfAnswers++;
                i++;
            }
        }
    }
}