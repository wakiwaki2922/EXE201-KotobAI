interface TestimonialCardProps {
  name: string;
  feedback: string;
  role: string;
}

export function TestimonialCard({ name, feedback, role }: TestimonialCardProps) {
    return (
      <div className="bg-white p-6 rounded-lg shadow-md text-left">
        <p className="text-gray-800 italic">&rdquo;{feedback}&rdquo;</p>
        <div className="mt-4">
          <h5 className="font-bold text-blue-600">{name}</h5>
          <p className="text-sm text-gray-500">{role}</p>
        </div>
      </div>
    );
  }
  