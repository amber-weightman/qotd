import { Container, Link, Typography } from "@mui/material";

function About() {

  return (
    <Container maxWidth={false} >
      <Typography variant="h5" component="h1">
        About
      </Typography>
      <Typography variant="h6" component="h2">
        About QOTD
      </Typography>
      <Typography paragraph={true} variant={"body1"}>
        Just a little app for AI-generating questions of the day
      </Typography>
      <Typography paragraph={true}>
        API:
        <Link href="https://questionoftheday.azurewebsites.net/swagger" target="_blank" rel="noopener">
          https://questionoftheday.azurewebsites.net/swagger
        </Link>
      </Typography>
      <Typography paragraph={true} variant={"body1"}>
        This app uses cookies
      </Typography>
      <Typography variant="h6" component="h2">
        About me
      </Typography>
      <Typography paragraph={true}>
        GitHub:       
        <Link href="https://github.com/amber-weightman/qotd" target="_blank" rel="noopener">
          https://github.com/amber-weightman/qotd
        </Link>
      </Typography>
      <Typography paragraph={true}>
        LinkedIn:
        <Link href="https://www.linkedin.com/in/amberweightman" target="_blank" rel="noopener">
          https://www.linkedin.com/in/amberweightman
        </Link>
      </Typography>
    </Container>
  );
}

export default About;