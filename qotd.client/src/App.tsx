import { Backdrop, Box, CircularProgress, CssBaseline } from '@mui/material';
import { ThemeProvider, createTheme } from '@mui/material/styles';
import { useEffect, useState } from 'react';
import './App.css';
import AppMenu from './components/AppMenu';
import QuestionDisplay from './components/QuestionDisplay';
import RateLimitDialog from './components/RateLimitDialog';

const darkTheme = createTheme({
  palette: {
    mode: 'dark',
  },
  //typography: {
  //  fontFamily: [
  //    '-apple-system',
  //    'BlinkMacSystemFont',
  //    '"Segoe UI"',
  //    'Roboto',
  //    '"Helvetica Neue"',
  //    'Arial',
  //    'sans-serif',
  //    '"Apple Color Emoji"',
  //    '"Segoe UI Emoji"',
  //    '"Segoe UI Symbol"',
  //  ].join(','),
  //},
});

function App() {
  const [questionId, setQuestionId] = useState<string>();
  const [question, setQuestion] = useState<string>();
  const [loading, setLoading] = useState(true);
  const [initialised, setInitialised] = useState(false);
  const [rateLimited, setRateLimited] = useState(false);

  const apiKey: string | undefined = import.meta.env.VITE_API_KEY;  
  console.log("API Key " + apiKey);
  console.log(import.meta.env.VITE_SOME_KEY) 

  useEffect(() => {
    //let ignore = false; // https://react.dev/learn/synchronizing-with-effects#how-to-handle-the-effect-firing-twice-in-development
    init();
  }, [initialised]);

  useEffect(() => {
    handleGenerateQuestion();
  }, [initialised]);

  useEffect(() => {
    handleLookupQuestion();
  }, [questionId]);

  const init = async () => {
    if (initialised) {
      return;
    }

    const request: Request = new Request('question/setup', apiKey == undefined ? undefined : { headers: [['x-api-key', apiKey]] });
    const response = await fetch(request);

    if (!response.ok) {
      console.log('Something went wrong');
      if (response.status === 429) {
        setRateLimited(true);
      }
      return;
    }
    setInitialised(true);
  }

  const handleGenerateQuestion = async () => {
    if (!initialised) {
      return;
    }

    const request: Request = new Request('question/generate-question', apiKey == undefined ? undefined : { headers: [['x-api-key', apiKey]] });
    const response = await fetch(request);

    if (!response.ok) {
      console.log('Something went wrong');
      if (response.status === 429) {
        setRateLimited(true);
      }
      return;
    }
   
    const r: string = await response.text();

    setQuestionId(r);
  }

  const handleLookupQuestion = async () => {
    if (!initialised || !questionId) {
      return;
    }

    const request: Request = new Request('question/get-question/' + questionId, apiKey == undefined ? undefined : { headers: [['x-api-key', apiKey]] });
    const response = await fetch(request);

    if (!response.ok) {
      console.log('Something went wrong');
      if (response.status === 429) {
        setRateLimited(true);
      }
      return;
    }

    const data: string = await response.text();

    setQuestion(data);
    console.log(data);
    setLoading(false);
  }

  const handleLookupNewQuestion = async () => {
    setLoading(true);
    setQuestion(undefined);
    handleGenerateQuestion();
  }

  return (
    <ThemeProvider theme={darkTheme}>
      <CssBaseline />
      <RateLimitDialog open={rateLimited} />
        <Box>
          {question && <QuestionDisplay question={question}></QuestionDisplay>}
          <Backdrop
            open={loading}
          >
            <CircularProgress color="inherit" />
          </Backdrop>
          <AppMenu question={question} lookupQuestion={handleLookupNewQuestion} />
        </Box>
    </ThemeProvider>
  );
}

export default App;