import { useEffect, useState } from 'react';
import './App.css';
import { authApi, challengesApi, coursesApi, submissionsApi, tagsApi } from './services/api';

const initialChallenge = {
  title: '',
  prompt: '',
  correctAnswer: '',
  maxPoints: 10,
  maxAttempts: 3,
  isPublished: true,
  courseId: '',
  tagIds: [],
};

function App() {
  const [token, setToken] = useState(localStorage.getItem('token'));
  const [login, setLogin] = useState({ username: 'student', password: 'Pass123!' });
  const [courses, setCourses] = useState([]);
  const [tags, setTags] = useState([]);
  const [challenges, setChallenges] = useState([]);
  const [filter, setFilter] = useState({ search: '', courseId: '', tagId: '', publishedOnly: false });
  const [newChallenge, setNewChallenge] = useState(initialChallenge);
  const [submission, setSubmission] = useState({ challengeId: '', studentName: '', answer: '' });
  const [message, setMessage] = useState('');
  const [error, setError] = useState('');

  useEffect(() => {
    if (!token) return;

    Promise.all([coursesApi.getAll(), tagsApi.getAll()])
      .then(([courseData, tagData]) => {
        setCourses(courseData);
        setTags(tagData);
      })
      .catch((err) => setError(err.message));
  }, [token]);

  useEffect(() => {
    if (!token) return;
    loadChallenges();
  }, [token, filter]);

  const loadChallenges = async () => {
    try {
      setError('');
      const data = await challengesApi.getAll(filter);
      setChallenges(data);
    } catch (err) {
      setError(err.message);
    }
  };

  const onLogin = async (e) => {
    e.preventDefault();
    try {
      const data = await authApi.login(login.username, login.password);
      localStorage.setItem('token', data.token);
      setToken(data.token);
      setMessage('Logged in');
      setError('');
    } catch (err) {
      setError(err.message);
    }
  };

  const onCreateChallenge = async (e) => {
    e.preventDefault();
    try {
      await challengesApi.create({
        ...newChallenge,
        courseId: Number(newChallenge.courseId),
        maxPoints: Number(newChallenge.maxPoints),
        maxAttempts: Number(newChallenge.maxAttempts),
        tagIds: newChallenge.tagIds.map(Number),
      });
      setNewChallenge(initialChallenge);
      setMessage('Challenge created');
      loadChallenges();
    } catch (err) {
      setError(err.message);
    }
  };

  const onSubmitAnswer = async (e) => {
    e.preventDefault();
    try {
      const result = await submissionsApi.submit({
        challengeId: Number(submission.challengeId),
        studentName: submission.studentName,
        answer: submission.answer,
      });
      setMessage(`${result.message} (${result.awardedPoints} points)`);
      setSubmission({ challengeId: '', studentName: '', answer: '' });
      setError('');
    } catch (err) {
      setError(err.message);
    }
  };

  const logout = () => {
    localStorage.removeItem('token');
    setToken(null);
    setChallenges([]);
    setMessage('Logged out');
  };

  if (!token) {
    return (
      <main className="container">
        <h1>Subapp 2: API + SPA</h1>
        <p>Log in to manage interactive challenges.</p>
        <form className="panel" onSubmit={onLogin}>
          <label>Username <input required value={login.username} onChange={(e) => setLogin({ ...login, username: e.target.value })} /></label>
          <label>Password <input required type="password" value={login.password} onChange={(e) => setLogin({ ...login, password: e.target.value })} /></label>
          <button>Login</button>
        </form>
        {error && <p className="error">{error}</p>}
      </main>
    );
  }

  return (
    <main className="container">
      <header className="headerRow">
        <h1>Interactive Challenge Dashboard</h1>
        <button onClick={logout}>Logout</button>
      </header>

      {message && <p className="success">{message}</p>}
      {error && <p className="error">{error}</p>}

      <section className="panel">
        <h2>Search & filter challenges</h2>
        <div className="grid4">
          <input placeholder="Search text" value={filter.search} onChange={(e) => setFilter({ ...filter, search: e.target.value })} />
          <select value={filter.courseId} onChange={(e) => setFilter({ ...filter, courseId: e.target.value })}>
            <option value="">All courses</option>
            {courses.map((course) => <option key={course.id} value={course.id}>{course.title}</option>)}
          </select>
          <select value={filter.tagId} onChange={(e) => setFilter({ ...filter, tagId: e.target.value })}>
            <option value="">All tags</option>
            {tags.map((tag) => <option key={tag.id} value={tag.id}>{tag.name}</option>)}
          </select>
          <label><input type="checkbox" checked={filter.publishedOnly} onChange={(e) => setFilter({ ...filter, publishedOnly: e.target.checked })} /> Published only</label>
        </div>
      </section>

      <section className="panel">
        <h2>Create challenge</h2>
        <form onSubmit={onCreateChallenge} className="grid2">
          <input required placeholder="Title" value={newChallenge.title} onChange={(e) => setNewChallenge({ ...newChallenge, title: e.target.value })} />
          <select required value={newChallenge.courseId} onChange={(e) => setNewChallenge({ ...newChallenge, courseId: e.target.value })}>
            <option value="">Select course</option>
            {courses.map((course) => <option key={course.id} value={course.id}>{course.title}</option>)}
          </select>
          <textarea required placeholder="Prompt" value={newChallenge.prompt} onChange={(e) => setNewChallenge({ ...newChallenge, prompt: e.target.value })} />
          <input required placeholder="Correct answer" value={newChallenge.correctAnswer} onChange={(e) => setNewChallenge({ ...newChallenge, correctAnswer: e.target.value })} />
          <input required type="number" min="1" max="100" value={newChallenge.maxPoints} onChange={(e) => setNewChallenge({ ...newChallenge, maxPoints: e.target.value })} />
          <input required type="number" min="1" max="10" value={newChallenge.maxAttempts} onChange={(e) => setNewChallenge({ ...newChallenge, maxAttempts: e.target.value })} />
          <select multiple value={newChallenge.tagIds} onChange={(e) => setNewChallenge({ ...newChallenge, tagIds: [...e.target.selectedOptions].map((o) => o.value) })}>
            {tags.map((tag) => <option key={tag.id} value={tag.id}>{tag.name}</option>)}
          </select>
          <label><input type="checkbox" checked={newChallenge.isPublished} onChange={(e) => setNewChallenge({ ...newChallenge, isPublished: e.target.checked })} /> Published</label>
          <button>Create</button>
        </form>
      </section>

      <section className="panel">
        <h2>Challenge list</h2>
        <p>{challenges.length === 0 ? 'No challenges match your filters.' : `${challenges.length} challenge(s) found.`}</p>
        <ul>
          {challenges.map((challenge) => (
            <li key={challenge.id}>
              <strong>{challenge.title}</strong> — {challenge.courseTitle} — {challenge.isPublished ? 'Published' : 'Draft'}
              {challenge.tags.length > 0 && <span> ({challenge.tags.join(', ')})</span>}
            </li>
          ))}
        </ul>
      </section>

      <section className="panel">
        <h2>Submit challenge answer</h2>
        <form onSubmit={onSubmitAnswer} className="grid2">
          <select required value={submission.challengeId} onChange={(e) => setSubmission({ ...submission, challengeId: e.target.value })}>
            <option value="">Select challenge</option>
            {challenges.filter((c) => c.isPublished).map((challenge) => <option key={challenge.id} value={challenge.id}>{challenge.title}</option>)}
          </select>
          <input required placeholder="Student name" value={submission.studentName} onChange={(e) => setSubmission({ ...submission, studentName: e.target.value })} />
          <textarea required placeholder="Your answer" value={submission.answer} onChange={(e) => setSubmission({ ...submission, answer: e.target.value })} />
          <button>Submit answer</button>
        </form>
      </section>
    </main>
  );
}

export default App;
