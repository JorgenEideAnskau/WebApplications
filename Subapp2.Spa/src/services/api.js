const API_BASE = 'https://localhost:7082/api';

async function request(path, options = {}) {
  const token = localStorage.getItem('token');
  const headers = {
    'Content-Type': 'application/json',
    ...(options.headers || {}),
  };

  if (token) {
    headers.Authorization = 'Bearer ' + token;
  }

  const response = await fetch(`${API_BASE}${path}`, { ...options, headers });
  if (response.status === 401) {
    throw new Error('Unauthorized. Please log in again.');
  }

  if (!response.ok) {
    const payload = await response.json().catch(() => ({}));
    throw new Error(payload.message || `Request failed with status ${response.status}`);
  }

  if (response.status === 204) {
    return null;
  }

  return response.json();
}

export const authApi = {
  login: (username, password) => request('/auth/login', { method: 'POST', body: JSON.stringify({ username, password }) }),
};

export const coursesApi = {
  getAll: () => request('/courses'),
};

export const tagsApi = {
  getAll: () => request('/tags'),
};

export const challengesApi = {
  getAll: ({ search, courseId, tagId, publishedOnly }) => {
    const params = new URLSearchParams();
    if (search) params.set('search', search);
    if (courseId) params.set('courseId', courseId);
    if (tagId) params.set('tagId', tagId);
    if (publishedOnly) params.set('publishedOnly', 'true');
    const query = params.toString();
    return request(`/challenges${query ? `?${query}` : ''}`);
  },
  create: (challenge) => request('/challenges', { method: 'POST', body: JSON.stringify(challenge) }),
};

export const submissionsApi = {
  submit: (submission) => request('/submissions', { method: 'POST', body: JSON.stringify(submission) }),
};
