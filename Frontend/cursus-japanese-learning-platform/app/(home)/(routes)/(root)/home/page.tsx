"use client";

import { useEffect, useState } from "react";
import Cookies from "js-cookie";
import SearchInput from "@/components/search-input";
import { isLoggedIn } from "@/lib/login-check";
import { TestimonialCard } from "@/components/testimonial-card";
import { Button } from "@/components/ui/button";

const Home = () => {
  const token = Cookies.get("jwtToken");
  const [searchResults, setSearchResults] = useState<any>(null);

  const CourseCard = ({ title, description }: { title: string; description: string }) => (
    <div className="bg-white p-6 rounded-lg shadow-lg">
      <h4 className="text-xl font-bold text-blue-600 mb-2">{title}</h4>
      <p className="text-gray-700 mb-4">{description}</p>
      <Button variant="outline">Learn More</Button>
    </div>
  );

  // Kiểm tra trạng thái đăng nhập
  if (token) {
    isLoggedIn();
  }

  useEffect(() => {
    // Lắng nghe sự kiện tùy chỉnh "searchUpdated" để cập nhật kết quả tìm kiếm
    const handleSearchUpdate = (event: any) => {
      const data = event.detail;
      setSearchResults(data);
    };

    window.addEventListener("searchUpdated", handleSearchUpdate);

    return () => {
      window.removeEventListener("searchUpdated", handleSearchUpdate);
    };
  }, []);

  return (
    <>
      <div className="px-6 pt-6 md:hidden md:mb-0 block">
        {/* Component tìm kiếm */}
        {/* <SearchInput /> */}
      </div>
      <div className="p-6 space-y-4">
        {/* Hiển thị kết quả tìm kiếm nếu có */}
        {searchResults ? (
          <div>
            <p className="text-lg font-medium mb-4">
              Search results for <strong>{searchResults.word}</strong>
            </p>
            <div className="flex gap-3">
              <div className="bg-white p-4 rounded-lg shadow-md space-y-2 flex-grow">
                <p className="text-sm text-gray-700">{searchResults.word}</p>
                <p className="text-sm text-gray-700">Meaning: {searchResults.meaning}</p>
              </div>
              <img
                width={200}
                src="https://th.bing.com/th/id/R.aff4ceb15704422ff22c17caa5529307?rik=s%2faef%2fupI7VcJQ&pid=ImgRaw&r=0"
                alt="Illustration"
              />
            </div>
          </div>
        ) : (
          <>
            {/* Hero Section */}
            <section className="bg-blue-600 text-white py-20 text-center">
              <h2 className="text-4xl font-bold mb-4">Master Japanese with Cursus</h2>
              <p className="text-lg mb-6">Explore a new language and culture with our immersive Japanese courses</p>
              <Button className="bg-white text-blue-600 hover:bg-gray-200">Get Started</Button>
            </section>

            {/* Courses Section */}
            <section id="courses" className="py-16">
              <div className="container mx-auto text-center">
                <h3 className="text-3xl font-bold mb-8 text-gray-800">Our Packages</h3>
                <div className="grid grid-cols-1 md:grid-cols-3 gap-8">
                  <CourseCard title="Beginner Japanese" description="Learn the basics and start your Japanese journey." />
                  <CourseCard title="Intermediate Japanese" description="Expand your knowledge and improve your fluency." />
                  <CourseCard title="Advanced Japanese" description="Master Japanese and communicate like a native." />
                </div>
              </div>
            </section>

            {/* Testimonials Section */}
            <section id="testimonials" className="py-16 bg-gray-100">
              <div className="container mx-auto text-center">
                <h3 className="text-3xl font-bold mb-8 text-gray-800">What Our Students Say</h3>
                <div className="grid grid-cols-1 md:grid-cols-2 gap-8">
                  <TestimonialCard 
                    name="John Doe"
                    feedback="Cursus has transformed my language skills!"
                    role="Student"
                  />
                  <TestimonialCard 
                    name="Jane Smith"
                    feedback="The packages are interactive and very engaging!"
                    role="Student"
                  />
                </div>
              </div>
            </section>

            {/* Footer */}
            <footer className="py-6 bg-blue-600 text-white text-center">
              <p>&copy; 2024 Cursus. All rights reserved.</p>
            </footer>
          </>
        )}
      </div>  
    </>
  );
};

export default Home;
