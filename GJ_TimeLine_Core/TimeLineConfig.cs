using Sprache;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GJ_TimeLine_Core
{
    public class AlertAll
    {
        public string ActivityName;
        public double ReminderTime;
        public string AlertSound;
    }
    public class TimelineActivityData
    {
        public double time;
        public string value;

    }
    public class TimelineConfig
    {
        public List<TimelineActivityData> Items;
        public List<AlertAll> AlertAlls;
        public List<string> HideAlls;

        public TimelineConfig()
        {
            Items = new List<TimelineActivityData>();
            AlertAlls = new List<AlertAll>();
            HideAlls = new List<string>();
        }
    }
    public class TConfigParser
    {
        public delegate void ConfigOp(TimelineConfig config);
        public struct SyncWindowSettings
        {
            public double WindowBefore;
            public double WindowAfter;
        };
        const int TRASH = 0;

        static readonly Parser<Double> DecimalDouble = Parse.Decimal.Select(str => Double.Parse(str, CultureInfo.InvariantCulture));
        static readonly Parser<string> QuotedString =
                from quoteStart in Parse.Char('"')
                from quotedString in Parse.CharExcept('"').Many().Text()
                from quoteEnd in Parse.Char('"')
                select quotedString;

        static readonly Parser<int> Comment = Parse.Regex(@"^#.*").Return(TRASH);
        static readonly Parser<string> Spaces = Parse.Regex(@"^[ 　]+");
        static readonly Parser<string> NonWhiteSpaces = Parse.Char(c => !char.IsWhiteSpace(c), "non whitespace char").Many().Text();
        static readonly Parser<string> MaybeQuotedString = QuotedString.XOr(NonWhiteSpaces);


        static readonly Parser<char> RegexEscapedSlash =
                   from escape in Parse.Char('\\')
                   from slash in Parse.Char('/')
                   select '/';
        static readonly Parser<char> RegexChar = RegexEscapedSlash.Or(Parse.CharExcept('/'));
        static readonly Parser<string> Regex =
            from slash in Parse.Char('/')
            from regex in RegexChar.Many().Text()
            from slash2 in Parse.Char('/')
            select regex;


        static readonly Parser<Tuple<double, string>> TimelineActivity =
            from timeFromStart in Parse.Decimal
            from spaces in Spaces
            from name in MaybeQuotedString
            select new Tuple<double, string>(double.Parse(timeFromStart, CultureInfo.InvariantCulture), name);

        static readonly Parser<ConfigOp> TimelineActivityStatement =
          TimelineActivity.Select<Tuple<double, string>, ConfigOp>(t => ((TimelineConfig config) =>
          {
              TimelineActivityData tad = new TimelineActivityData();
              tad.time = t.Item1;
              tad.value = t.Item2;

              config.Items.Add(tad);
          })).Named("TimelineActivityStatement");
        static readonly Parser<Tuple<string, string, string>> AlertAll =
            from alertall in Parse.String("alertall")
            from spaces in Spaces
            from activityName in MaybeQuotedString
            from spaces2 in Spaces
            from before in Parse.String("before")
            from spaces3 in Spaces
            from reminderTime in Parse.Decimal
            from spaces4 in Spaces
            from sound_keyword in Parse.String("sound")
            from spaces5 in Spaces
            from soundName in MaybeQuotedString
            select new Tuple<string, string, string>(activityName, reminderTime, soundName);

        static readonly Parser<int> LineBreak = Parse.Or(Parse.Char('\n'), Parse.Char('\r')).AtLeastOnce().Return(TRASH);

        static readonly Parser<ConfigOp> AlertAllStatement =
    AlertAll.Select<Tuple<string, string, string>, ConfigOp>((Tuple<string, string, string> t) => ((TimelineConfig config) =>
    {
        var alertSound = t.Item3;
        config.AlertAlls.Add(new AlertAll { ActivityName = t.Item1, ReminderTime = Double.Parse(t.Item2), AlertSound = alertSound });
    }));
        static readonly Parser<int> StatementSeparator =
    from beforeSpaces in Parse.Optional(Spaces)
    from comment in Parse.Optional(Comment)
    from lb in LineBreak
    from afterSpaces in Parse.Optional(Spaces)
    select TRASH;



        static readonly Parser<string> HideAll =
    from hideall in Parse.String("hideall")
    from spaces in Spaces
    from activityName in MaybeQuotedString
    select activityName;

        static readonly Parser<ConfigOp> HideAllStatement =
    HideAll.Select<string, ConfigOp>((string targetActivityName) => ((TimelineConfig config) =>
    {
        config.HideAlls.Add(targetActivityName);
    }));

        static readonly Parser<ConfigOp> EmptyStatement = Parse.Return<ConfigOp>((TimelineConfig config) => { });


        static readonly Parser<ConfigOp> TimelineStatement =
           TimelineActivityStatement.Or(AlertAllStatement).Or(HideAllStatement).Or(EmptyStatement);

        static readonly Parser<IEnumerable<ConfigOp>> TimelineStatements =
     from stmts in TimelineStatement.DelimitedBy(StatementSeparator.Many())
     select stmts;
        public static readonly Parser<TimelineConfig> TimelineConfig = TimelineStatements.Select(stmts =>
        {
            TimelineConfig config = new TimelineConfig();
            foreach (ConfigOp op in stmts)
                op(config);
            return config;
        }).End();
    }

}
