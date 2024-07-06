import { Backdrop, Box, CircularProgress, CssBaseline } from '@mui/material';
import { ThemeProvider, createTheme } from '@mui/material/styles';
import { useEffect, useState } from 'react';
import './App.css';
import AppMenu from './components/AppMenu';
import QuestionDisplay from './components/QuestionDisplay';

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

  useEffect(() => {
    //let ignore = false; // https://react.dev/learn/synchronizing-with-effects#how-to-handle-the-effect-firing-twice-in-development
    init();

  }, [initialised]);

  useEffect(() => {
    init2();
  }, [initialised]);

  useEffect(() => {
    lookupInitialQuestion();
  }, [questionId]);

  const init = async () => {
    if (initialised) {
      return;
    }

    await fetch('question/setup');
    setInitialised(true);
  }

  const init2 = async () => {
    if (!initialised || questionId) {
      return;
    }

    const response = await fetch('question/generate-question');
    const r: string = await response.text();
    setQuestionId(r);
  }

  const lookupInitialQuestion = async () => {
    if (!initialised) {
      return;
    }

    if (!questionId) {
      return;
    }

    const response = await fetch('question/get-question/' + questionId);

    const data: string = await response.text();
    setQuestion(data);
    console.log(data);
    setLoading(false);
  }

  const handleLookupQuestion = async () => {
    if (!initialised) {
      return;
    }

    const response1 = await fetch('question/generate-question');
    const r: string = await response1.text();
    setQuestionId(r);

    if (!questionId) {
      return;
    }

    setLoading(true);
    setQuestion(undefined);


    const response = await fetch('question/get-question/' + questionId);
    const data: string = await response.text();
    setQuestion(data);
    console.log(data);
    setLoading(false);
  }

  return (
    <ThemeProvider theme={darkTheme}>
      <CssBaseline />
        <Box>
          {question && <QuestionDisplay question={question}></QuestionDisplay>}
          <Backdrop
            open={loading}
          >
            <CircularProgress color="inherit" />
          </Backdrop>
          <AppMenu question={question} lookupQuestion={handleLookupQuestion} />
        </Box>
    </ThemeProvider>

  );
}

export default App;